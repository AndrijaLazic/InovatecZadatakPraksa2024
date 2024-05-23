using DOMAIN.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DOMAIN.Models.VehicleReservation;

namespace DOMAIN.Abstraction
{
    public interface IVehicleService
    {
        public List<IVehicle> GetVehicles();
        public void RentVehicle(Reservation reservation, int vehicleID);

        public IVehicle? GetVehicle(int vehicleID);
    }
}
