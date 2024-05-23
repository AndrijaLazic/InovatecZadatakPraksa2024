using BLL.Services;
using CsvHelper;
using DOMAIN.Abstraction;
using DOMAIN.Enums;
using DOMAIN.Models;
using FluentAssertions;
using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestProject.DAL_test;

namespace TestProject.MAIN_test
{
    public class MainTest
    {
        private VehicleService _vehicleService;
        private CustomerService _customerService;

        public MainTest()
        {
            CSVModuleTest cSVModuleTest = new CSVModuleTest();
            _vehicleService = new VehicleService(cSVModuleTest.csvModule, new VehicleFactory(), "vozila.csv", "oprema.csv", "vozilo_oprema.csv");
            _customerService = new CustomerService(cSVModuleTest.csvModule, "kupci.csv", "zahtevi_za_rezervacije.csv", "rezervacije.csv");
        }

        [Fact]
        public void RentVehicle()
        {
            List<dynamic> reservations = _customerService.GetNewCustomersReservations();
            List<Customer> customers = _customerService.GetCustomers();
            DateTime currentTime = DateTime.Now;
            for (int i = 0; i < reservations.Count; i++)
            {
                Customer currentCustomer = customers.Where(x => x.id == int.Parse(reservations[i].KupacId)).First();

                Reservation reservation = new Reservation()
                {
                    timeOfOrder = DateTime.Parse(reservations[i].DatumDolaska),
                    customer = currentCustomer,
                    EndDate = DateTime.Parse(reservations[i].KrajRezervacije),
                    StartDate = DateTime.Parse(reservations[i].PocetakRezervacije)
                };

                if(currentCustomer.id == 8)
                {
                    Action action = () => _vehicleService.RentVehicle(reservation, int.Parse(reservations[i].VoziloId));
                    action.Should().Throw<Exception>().Where(e => e.Message.StartsWith("User:Test2 Test2 doesn`t have enought cash assets"));
                    continue;
                }
                
                _vehicleService.RentVehicle(reservation, int.Parse(reservations[i].VoziloId));
            }

            List<IVehicle> vehicles = _vehicleService.GetVehicles();
    
            vehicles.Where(x=>x.id==4).First().reservation.getReservations().Where(x=>x.status==ReservationStatus.Success).Count().Should().Be(3);
            vehicles.Where(x => x.id == 2).First().reservation.getReservations().Where(x => x.status == ReservationStatus.Success).Count().Should().Be(2);
            vehicles.Where(x => x.id == 6).First().reservation.getReservations().Where(x => x.status == ReservationStatus.Success).Count().Should().Be(1);

            customers.Where(x => x.id == 7).First().cashAssets.Should().Be(14m);
            customers.Where(x => x.id == 8).First().cashAssets.Should().Be(190m);
        }
    }
}
