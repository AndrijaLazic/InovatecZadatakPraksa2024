using DOMAIN.Abstraction;
using DOMAIN.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DOMAIN.Models
{
    public class Car:IVehicle
    {
        public int kmsDriven { get; set; }
        public CarBodyType carBodyType { get; set; }
        public List<VehicleEquipment> equipment=new List<VehicleEquipment>();
        public decimal priceOfEquipment { get; set; } = 0m;

        public override decimal GetTotalPrice()
        {
            return this.price + this.priceOfEquipment;
        }

        public override string ToString()
        {
            return base.ToString() + " price:"+ GetTotalPrice() + " kmsDriven:" + this.kmsDriven + " carBodyType:" + this.carBodyType.ToString();
        }
    }
}
