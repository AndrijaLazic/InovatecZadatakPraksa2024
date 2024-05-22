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

        public VehicleServiceTest()
        {
            CSVModuleTest cSVModuleTest = new CSVModuleTest();
            _vehicleService = new VehicleService(cSVModuleTest.csvModule, new VehicleFactory(), "vozila.csv", "oprema.csv", "vozilo_oprema.csv");
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

        
    }
}
