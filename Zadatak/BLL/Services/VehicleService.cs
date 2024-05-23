using DAL;
using DOMAIN.Abstraction;
using DOMAIN.Enums;
using DOMAIN.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DOMAIN.Models.VehicleReservation;

namespace BLL.Services
{
    public class VehicleService:IVehicleService
    {
        private CSVModule _csvModule;
        private VehicleFactory _vehicleFactory;
        private string _fileNameVehicles;
        private string _fileNameEquipment;
        private string _fileNameVehicleEquipment;
        private string _fileNameNewCustomersReservations;
        private string _fileNameOldCustomersReservations;
        private Dictionary<int, IVehicle> _vehicles;
        private Dictionary<int, VehicleEquipment> _equipment;
        private Dictionary<int, object> _vehiclesLocks;
        private ICustomerService _customerService;

        private List<dynamic> oldReservationsCSV;

        public VehicleService(CSVModule csvModule, VehicleFactory vehicleFactory, AppConfig appConfig, ICustomerService customerService)
        {

            _csvModule = csvModule;
            _vehicleFactory = vehicleFactory;
            _fileNameVehicles = appConfig.csvConfig.fileNameVehicles;
            _fileNameEquipment = appConfig.csvConfig.fileNameEquipment;
            _fileNameVehicleEquipment = appConfig.csvConfig.fileNameVehicleEquipment;
            _fileNameOldCustomersReservations = appConfig.csvConfig.fileNameOldCustomersReservations;
            _fileNameNewCustomersReservations = appConfig.csvConfig.fileNameNewCustomersReservations;
            _vehicles = new Dictionary<int, IVehicle>();
            _equipment = new Dictionary<int, VehicleEquipment>();
            _customerService=customerService;

            Task[] tasks = new Task[3]
            {
                Task.Run(() =>
                {
                    LoadVehicles();
                }),
                Task.Run(() =>
                {
                    LoadEquipment();
                }),
                Task.Run(() =>
                {
                    LoadOldReservationsFromFile();
                })
            };
            Task.WaitAll(tasks);
            AddEquipmentToVehicles();
            LoadOldReservationsIntoVehicles();



            List<IVehicle> vehicles = _vehicles.Values.ToList();
            _vehiclesLocks = new Dictionary<int, object>();
            for (int i = 0; i < vehicles.Count; i++)
            {
                _vehiclesLocks.Add(vehicles[i].id, new object());
                if (vehicles[i] is Car)
                {
                    Car currentCar = ((Car)vehicles[i]);
                    for (int j = 0; j < currentCar.equipment.Count; j++)
                    {
                        if (currentCar.equipment[j].increasesPrice)
                        {
                            currentCar.priceOfEquipment = currentCar.priceOfEquipment + currentCar.equipment[j].price;
                            continue;
                        }
                        currentCar.priceOfEquipment = currentCar.priceOfEquipment - currentCar.equipment[j].price;
                    }
                }
            }
            _customerService = customerService;
        }

        public List<IVehicle> GetVehicles()
        {
            return _vehicles.Values.ToList();
        }

        private void LoadVehicles()
        {
            List<dynamic> readVehicles = _csvModule.ReadFile(_fileNameVehicles);
            

            for (int i = 0; i < readVehicles.Count; i++)
            {
                IVehicle newVehicle = _vehicleFactory.GetVehicle(readVehicles[i]);
                newVehicle.GetTotalPrice();
                _vehicles.TryAdd(newVehicle.id, newVehicle!);
            }
        }

        private void AddEquipmentToVehicles()
        {
            List<dynamic> readVehicleEquipment = _csvModule.ReadFile(_fileNameVehicleEquipment);
            for (int j = 0; j < readVehicleEquipment.Count; j++)
            {
                int vehicleID = int.Parse(readVehicleEquipment[j].VoziloId);
                int equipmentID = int.Parse(readVehicleEquipment[j].OpremaId);
                if (!_vehicles.TryGetValue(vehicleID, out IVehicle? vehicle))
                    throw new Exception("Equipment with id: " + equipmentID + " doesn`t exist");
                if (vehicle is Car)
                {
                    if (!_equipment.TryGetValue(equipmentID, out VehicleEquipment? equipment))
                        throw new Exception("Equipment with id: " + equipmentID + " doesn`t exist");
                    
                    ((Car)vehicle).equipment.Add(equipment);
                }
            }
        }

        private void LoadEquipment()
        {
            List<dynamic> readEquipments = _csvModule.ReadFile(_fileNameEquipment);
            for (int i = 0; i < readEquipments.Count; i++)
            {
                VehicleEquipment newEquipment = VehicleEquipment.GetVehicleEquipment(readEquipments[i]);
                _equipment.TryAdd(newEquipment.id, newEquipment!);
            }
        }

        public void RentVehicle(Reservation reservation, int vehicleID)
        {
            _vehicles.TryGetValue(vehicleID, out IVehicle? wantedVehicle);
            if (wantedVehicle == null)
                throw new Exception("Vehicle with id:" + vehicleID + " doesn`t exist");

            decimal realPrice = wantedVehicle.GetTotalPrice() * reservation.customer.GetDiscount();
            
            if (realPrice > reservation.customer.cashAssets)
                throw new Exception("User:" + reservation.customer.name +" "+ reservation.customer.lastName + " doesn`t have enought cash assets(" + reservation.customer.cashAssets + "/" + realPrice + ")");

            reservation.price=realPrice;

            _vehiclesLocks.TryGetValue(wantedVehicle.id, out object? vehicleLock);
            lock (vehicleLock!)
            {
                wantedVehicle.reservation.addReservation(reservation);
            }
        }

        public IVehicle? GetVehicle(int vehicleID)
        {
            _vehicles.TryGetValue(vehicleID,out IVehicle? vehicle);
            return vehicle;
        }

        public List<dynamic> GetNewCustomersReservations()
        {
            List<dynamic> reservations = _csvModule.ReadFile(_fileNameNewCustomersReservations);
            for (int i = 0; i < reservations.Count; i++)
            {
                dynamic expando = new ExpandoObject();
                expando.VoziloId = reservations[i].VoziloId;
                expando.KupacId = reservations[i].KupacId;
                expando.DatumDolaska = DateTime.Parse(reservations[i].DatumDolaska).ToString("dd/MM/yyyy");
                expando.PocetakRezervacije = DateTime.Parse(reservations[i].PocetakRezervacije).ToString("dd/MM/yyyy");
                DateTime finnishDate = DateTime.Parse(reservations[i].PocetakRezervacije);
                finnishDate = finnishDate.AddDays(int.Parse(reservations[i].BrojDana));
                expando.KrajRezervacije = finnishDate.ToString("dd/MM/yyyy");
                reservations[i] = expando;
            }
            return reservations;
        }

        private void LoadOldReservationsFromFile()
        {
            oldReservationsCSV= _csvModule.ReadFile(_fileNameOldCustomersReservations);
        }

        private void LoadOldReservationsIntoVehicles()
        {
            List<Customer> customers = _customerService.GetCustomers();

            for (int i = 0; i < oldReservationsCSV.Count; i++)
            {
                Customer customer = _customerService.GetCustomerById(int.Parse(oldReservationsCSV[i].KupacId));
                IVehicle currentVehicle = this.GetVehicle(int.Parse(oldReservationsCSV[i].VoziloId));
                Reservation currentReservation = new Reservation()
                {
                    customer = customer,
                    EndDate = DateTime.Parse(oldReservationsCSV[i].KrajRezervacije),
                    StartDate = DateTime.Parse(oldReservationsCSV[i].PocetakRezervacije),
                };
                currentVehicle.reservation.AddOldReservation(currentReservation);
            }
        }
    }
}
