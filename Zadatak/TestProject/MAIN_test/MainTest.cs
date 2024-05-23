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
            _customerService = new CustomerService(cSVModuleTest.csvModule, cSVModuleTest.appConfig);
            _vehicleService = new VehicleService(cSVModuleTest.csvModule, new VehicleFactory(), cSVModuleTest.appConfig, _customerService);

        }

        [Fact]
        public void RentVehicle()
        {
            dynamic cashAssetsUser7 = _customerService.GetCustomerById(7).cashAssets;


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

                if(currentCustomer.id == 8)
                {
                    Action action = () => _vehicleService.RentVehicle(reservation, int.Parse(reservations[i].VoziloId));
                    action.Should().Throw<Exception>().Where(e => e.Message.StartsWith("User:Test2 Test2 doesn`t have enought cash assets"));
                    continue;
                }
                
                _vehicleService.RentVehicle(reservation, int.Parse(reservations[i].VoziloId));
            }

            List<IVehicle> vehicles = _vehicleService.GetVehicles();
    
            vehicles.Where(x=>x.id==4).First().reservation.getReservations().Where(x=>x.status==ReservationStatus.Success).Count().Should().Be(2);
            vehicles.Where(x => x.id == 2).First().reservation.getReservations().Where(x => x.status == ReservationStatus.Success).Count().Should().Be(2);

            List<Reservation> vehicle6Reservations = vehicles.Where(x => x.id == 6).First().reservation.getReservations();
            vehicle6Reservations.Where(x => x.status == ReservationStatus.Success).Count().Should().Be(1);

            _customerService.GetCustomerById(7).cashAssets.Should().Be(cashAssetsUser7- vehicle6Reservations.Where(x=>x.customer.id==7).First().price);
            _customerService.GetCustomerById(8).cashAssets.Should().Be(190m);
        }
    }
}
