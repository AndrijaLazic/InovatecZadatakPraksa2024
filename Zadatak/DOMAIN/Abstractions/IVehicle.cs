using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DOMAIN.Abstraction
{
    public class IVehicle
    {
        public int id { get; set; }
        public string? producer { get; set; }
        public decimal price { get; set; }
        public float fuelConsumption { get; set; }
        public string? model { get; set; }

        public virtual decimal GetTotalPrice() { return price; }
    }
}
