using DOMAIN.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DOMAIN.Abstraction
{
    public interface IVehicleService
    {
        public List<IVehicle> GetVehicles();
    }
}
