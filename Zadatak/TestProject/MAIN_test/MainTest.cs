using BLL.Services;
using CsvHelper;
using DAL;
using DOMAIN.Abstraction;
using DOMAIN.Enums;
using DOMAIN.Models;
using FluentAssertions;
using Microsoft.CSharp.RuntimeBinder;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TestProject.DAL_test;

namespace TestProject.MAIN_test
{
    public class MainTest
    {
        private VehicleService _vehicleService;
        private CustomerService _customerService;
        private CSVModule _csvModule;

        public MainTest()
        {
            
            CSVModuleTest cSVModuleTest = new CSVModuleTest();
            _csvModule=cSVModuleTest.csvModule;
            _customerService = new CustomerService(cSVModuleTest.csvModule, cSVModuleTest.appConfig);
            _vehicleService = new VehicleService(cSVModuleTest.csvModule, new VehicleFactory(), cSVModuleTest.appConfig, _customerService);
        }

        public void RentVehicle()
        {
            List<dynamic> reservations = _vehicleService.GetNewCustomersReservations();
            for (int i = 0; i < reservations.Count; i++)
            {
                Customer currentCustomer = _customerService.GetCustomerById(int.Parse(reservations[i].KupacId));

                Reservation reservation = new Reservation()
                {
                    timeOfOrder = DateTime.Parse(reservations[i].DatumDolaska),
                    customer = currentCustomer,
                    EndDate = DateTime.Parse(reservations[i].KrajRezervacije),
                    StartDate = DateTime.Parse(reservations[i].PocetakRezervacije)
                };

                //usecase not enough cash at start of request
                if(currentCustomer.id == 10)
                {
                    Action action = () => { 
                        _vehicleService.RentVehicle(reservation, int.Parse(reservations[i].VoziloId)); 
                    };
                    action.Should().Throw<Exception>().Where(e => e.Message.StartsWith("User:Test4 Test4 doesn`t have enought cash"));
                    continue;
                }
                else if(currentCustomer.id == 8 && int.Parse(reservations[i].VoziloId) == 6)
                {
                    Action action = () => {
                        _vehicleService.RentVehicle(reservation, int.Parse(reservations[i].VoziloId));
                    };
                    action.Should().Throw<Exception>().Where(e => e.Message.Equals("User:Test2 Test2 doesn`t have enought cash assets(194/198.00000) for vehicle:6"));
                    continue;
                }
                _vehicleService.RentVehicle(reservation, int.Parse(reservations[i].VoziloId));
                
                
            }

            List<IVehicle> vehicles = _vehicleService.GetVehicles();

            
        }

        public void writeNewReservations()
        {
            dynamic cashAssetsUser7 = _customerService.GetCustomerById(7).cashAssets;
            dynamic cashAssetsUser8 = _customerService.GetCustomerById(8).cashAssets;
            List<ReservationToWrite> successfullReservations = new List<ReservationToWrite>();

            List<IVehicle> vehicles = _vehicleService.GetVehicles();

            for (int i = 0; i < vehicles.Count; i++)
            {
                vehicles[i].reservation.getReservations(out List<Reservation> validReservations, out List<Reservation> invalidReservations);
                for (int j = 0; j < validReservations.Count; j++)
                {
                    successfullReservations.Add(ReservationToWrite.ConvertReservation(validReservations[j], vehicles[i].id));
                }

                switch (vehicles[i].id)
                {
                    case 2:
                        validReservations.Where(x => x.status == ReservationStatus.Success).Count().Should().Be(2);
                        break;
                    case 4:
                        validReservations.Where(x => x.status == ReservationStatus.Success).Count().Should().Be(3);
                        _customerService.GetCustomerById(8).cashAssets.Should().Be(cashAssetsUser8 - validReservations.Where(x => x.customer.id == 8).First().price);
                        break;
                    case 6:
                        validReservations.Count().Should().Be(1);
                        _customerService.GetCustomerById(7).cashAssets.Should().Be(cashAssetsUser7 - validReservations.Where(x => x.customer.id == 7).First().price);
                        break;
                    default:
                        // code block
                        break;
                }

            }

            _csvModule.WriteFile("nove_rezervacije.csv", successfullReservations);
        }

        [Fact]
        public void FinalOutput()
        {
            RentVehicle();
            writeNewReservations();
            List<Reservation> reservations = new List<Reservation>();
            List<dynamic> readReservations = _csvModule.ReadFile("nove_rezervacije.csv");

            readReservations.Count.Should().Be(9);
        }
    }
}
