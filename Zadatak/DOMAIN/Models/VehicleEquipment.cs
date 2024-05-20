using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DOMAIN.Models
{
    public class VehicleEquipment
    {
        public int id {  get; set; }
        public string? name { get; set; }
        public decimal price { get; set; }
        public bool increasesPrice { get; set; }

        public static VehicleEquipment GetVehicleEquipment(dynamic equipment)
        {
            VehicleEquipment newVehicleEquipment = new VehicleEquipment()
            {
                id = int.Parse(equipment.Id),
                name = equipment.Naziv,
                price = decimal.Parse(equipment.Cena),
                increasesPrice = int.Parse(equipment.PovecavaCenu) != 0,
            };
            return newVehicleEquipment;
        }
    }

    
}
