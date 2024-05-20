using DOMAIN.Abstraction;
using DOMAIN.Enums;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DOMAIN.Models
{
    public class VehicleFactory
    {
        public IVehicle GetVehicle(dynamic data)
        {
            var dict = (IDictionary<string, object>)data; 
            try
            {
                switch (data.TipVozila)
                {
                    case "Automobil":
                        Car car = new Car() { 
                            id= Int32.Parse(data.Id),
                            producer=data.Marka,
                            price= 200.00m,
                            fuelConsumption= float.Parse(data.Potrosnja),
                            kmsDriven= Int32.Parse(data.Kilometraza),
                            model=data.Model,
                            carBodyType= Enum.IsDefined(typeof(CarBodyType), data.Tip) ? Enum.Parse(typeof(CarBodyType), data.Tip) : CarBodyType.Undefined,
                        };
                        this.updatePriceCar(car);
                        return car;

                    case "Motor":
                        Motorcycle motorcycle = new Motorcycle() {
                            id = Int32.Parse(data.Id),
                            producer = data.Marka,
                            price = 100.00m,
                            fuelConsumption = float.Parse(data.Potrosnja),
                            enginePower= Int32.Parse(data.Snaga),
                            engineVolume = Int32.Parse(data.Kubikaza),
                            model = data.Model,
                            type= Enum.IsDefined(typeof(MotorcycleType), data.Tip) ? Enum.Parse(typeof(MotorcycleType), data.Tip) : MotorcycleType.Undefined,
                        };
                        this.updatePriceMotorcycle(motorcycle);
                        return motorcycle;
                    default:
                        throw new Exception("Error while creating vehicle: no vehicle type found:" + data.TipVozila);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        private Car updatePriceCar(Car car)
        {
            decimal startingPrice=car.price;
            switch (car.producer)
            {
                case "Mercedes":
                    if (car.kmsDriven < 50000)
                        car.price = car.price + startingPrice*0.06m;
                    if(car.carBodyType == CarBodyType.Limuzina)
                        car.price = car.price + startingPrice * 0.07m;
                    else if(car.carBodyType == CarBodyType.Hečbek && car.kmsDriven > 100000)
                        car.price = car.price - startingPrice * 0.03m;

                    return car;

                case "BMW":
                    if (car.fuelConsumption<7)
                        car.price = car.price + startingPrice * 0.15m;
                    else if(car.fuelConsumption>7)
                        car.price = car.price - startingPrice * 0.10m;
                    else
                        car.price = car.price - startingPrice * 0.15m;

                    return car;

                case "Peugeot":
                    if (car.carBodyType == CarBodyType.Limuzina)
                        car.price = car.price + startingPrice * 0.15m;
                    else if (car.carBodyType == CarBodyType.Karavan)
                        car.price = car.price + startingPrice * 0.20m;
                    else
                        car.price = car.price - startingPrice * 0.05m;

                    return car;
                default:
                    throw new Exception("Error while updating prices: Vehicle producer does not exist:" + car.producer);
            }
        }

        private Motorcycle updatePriceMotorcycle(Motorcycle motorcycle)
        {
            decimal startingPrice = motorcycle.price;
            switch (motorcycle.producer)
            {
                case "Harley":
                    motorcycle.price= motorcycle.price + startingPrice*0.15m;
                    if (motorcycle.engineVolume > 1200)
                        motorcycle.price = motorcycle.price + startingPrice * 0.10m;
                    else
                        motorcycle.price = motorcycle.price - startingPrice * 0.05m;

                    return motorcycle;
                case "Yamaha":
                    motorcycle.price = motorcycle.price + startingPrice * 0.10m;
                    if (motorcycle.enginePower > 180)
                        motorcycle.price = motorcycle.price + startingPrice * 0.05m;
                    else
                        motorcycle.price = motorcycle.price - startingPrice * 0.10m;

                    if (motorcycle.type == MotorcycleType.Heritage)
                        motorcycle.price = motorcycle.price + 50m;
                    else if (motorcycle.type == MotorcycleType.Sport)
                        motorcycle.price = motorcycle.price + 100m;
                    else
                        motorcycle.price = motorcycle.price - 10m;

                    return motorcycle;

                default:
                    throw new Exception("Error while updating prices: Vehicle producer does not exist:" + motorcycle.producer);
            }
        }
    }
}
