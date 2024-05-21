using DOMAIN.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DOMAIN.Models.VehicleReservation;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DOMAIN.Models
{
    public class Customer
    {
        public int id { get; set; }
        public string? name { get; set; }
        public string? lastName { get; set; }
        public decimal cashAssets { get; set; }
        public MembershipType membershipType { get; set; }

        public static Customer GetCustomer(dynamic dynamicCustomer)
        {
            Customer customer = new Customer()
            {
                id = int.Parse(dynamicCustomer.Id),
                name = dynamicCustomer.Ime,
                cashAssets = decimal.Parse(dynamicCustomer.Budzet),
                lastName = dynamicCustomer.Prezime,
                membershipType = Enum.IsDefined(typeof(MembershipType), dynamicCustomer.Clanarina) ? Enum.Parse(typeof(MembershipType), dynamicCustomer.Clanarina) : MembershipType.Undefined
            };
            return customer;
        }
    }
}
