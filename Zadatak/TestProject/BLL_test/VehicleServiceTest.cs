using BLL.Services;
using CsvHelper;
using DOMAIN.Abstraction;
using DOMAIN.Models;
using FluentAssertions;
using TestProject.DAL_test;

namespace TestProject.BLL_test
{
    public class VehicleServiceTest
    {
        private VehicleService _vehicleService;
        private CustomerService _customerService;
        public VehicleServiceTest()
        {
            CSVModuleTest cSVModuleTest = new CSVModuleTest();
            _customerService = new CustomerService(cSVModuleTest.csvModule, cSVModuleTest.appConfig);
            _vehicleService = new VehicleService(cSVModuleTest.csvModule, new VehicleFactory(), cSVModuleTest.appConfig, _customerService);
        }

        [Fact]
        public void GetVehicles_ReturnsListOfVehicles()
        {
            List<IVehicle> vehicles = _vehicleService.GetVehicles();
            vehicles.Should().HaveCount(7);
        }

        [Fact]
        public void CheckVehiclesPrices()
        {
            List<IVehicle> vehicles = _vehicleService.GetVehicles();
            vehicles.Where(x => x.id == 1).First().price.Should().Be(180);
            vehicles.Where(x => x.id == 2).First().price.Should().Be(212);
            vehicles.Where(x => x.id == 3).First().price.Should().Be(230);
            vehicles.Where(x => x.id == 4).First().price.Should().Be(215);
            vehicles.Where(x => x.id == 5).First().price.Should().Be(110);
            vehicles.Where(x => x.id == 6).First().price.Should().Be(190);
            vehicles.Where(x => x.id == 7).First().price.Should().Be(90);
        }

        [Fact]
        public void CheckEquipmentPrices()
        {
            List<IVehicle> vehicles = _vehicleService.GetVehicles();
            ((Car)vehicles.Where(x => x.id == 1).First()).priceOfEquipment.Should().Be(30m);
            ((Car)vehicles.Where(x => x.id == 2).First()).priceOfEquipment.Should().Be(50m);
            ((Car)vehicles.Where(x => x.id == 3).First()).priceOfEquipment.Should().Be(-20m);
            ((Car)vehicles.Where(x => x.id == 6).First()).priceOfEquipment.Should().Be(30m);
        }

        [Fact]
        public void CheckMembershipValidity()
        {
            IVehicle vehicle = _vehicleService.GetVehicles().Where(x => x.id == 4).First();
            List<dynamic> customersReservations = _vehicleService.GetNewCustomersReservations().Where(x => int.Parse(x.VoziloId) == 4 && "06-03-2024".Equals(x.DatumDolaska)).ToList();
            for (int i = 0; i < customersReservations.Count; i++)
            {
                Customer currentCustomer = _customerService.GetCustomerById(int.Parse(customersReservations[i].KupacId));

                Reservation reservation = new Reservation()
                {
                    timeOfOrder = DateTime.Parse(customersReservations[i].DatumDolaska),
                    customer = currentCustomer,
                    EndDate = DateTime.Parse(customersReservations[i].KrajRezervacije),
                    StartDate = DateTime.Parse(customersReservations[i].PocetakRezervacije)
                };

                _vehicleService.RentVehicle(reservation, vehicle.id);
            }
            vehicle.reservation.getReservations(out List<Reservation> valid, out List<Reservation> invalid);
            valid.Should().HaveCount(2);
            invalid.Should().HaveCount(2);
            valid[0].customer.id.Should().Be(7);
            valid[1].customer.id.Should().Be(9);
            valid[1].StartDate.ToString("dd-MM-yyyy").Should().Contain("17-07-2028");


        }
    }
}
