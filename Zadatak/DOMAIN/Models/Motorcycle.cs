using DOMAIN.Abstraction;
using DOMAIN.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DOMAIN.Models
{
    public class Motorcycle: IVehicle
    {
        public int engineVolume { get; set; }
        public int enginePower { get; set; }
        public MotorcycleType type { get; set; }

        public override string ToString()
        {
            return base.ToString() + " price:"+GetTotalPrice()+ " engineVolume:"+ engineVolume + " enginePower:"+ enginePower+ " type:" + type.ToString();
        }
    }
}
