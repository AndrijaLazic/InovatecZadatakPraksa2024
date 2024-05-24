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
            dynamic cashAssetsUser8 = _customerService.GetCustomerById(8).cashAssets;

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

            vehicles.Where(x => x.id == 4).First().reservation.getReservations(out List<Reservation> valid4Reservations, out List<Reservation> invalid4Reservations);
            valid4Reservations.Where(x => x.status == ReservationStatus.Success).Count().Should().Be(3);

            vehicles.Where(x => x.id == 2).First().reservation.getReservations(out List<Reservation> valid2Reservations, out List<Reservation> invalid2Reservations);
            valid2Reservations.Where(x => x.status == ReservationStatus.Success).Count().Should().Be(2);


            vehicles.Where(x => x.id == 6).First().reservation.getReservations(out List<Reservation> valid6Reservations, out List<Reservation> invalid6Reservations);

            valid6Reservations.Count().Should().Be(1);

            _customerService.GetCustomerById(7).cashAssets.Should().Be(cashAssetsUser7- valid6Reservations.Where(x=>x.customer.id==7).First().price);
            _customerService.GetCustomerById(8).cashAssets.Should().Be(cashAssetsUser8- valid4Reservations.Where(x => x.customer.id == 8).First().price);
        }
    }
}
