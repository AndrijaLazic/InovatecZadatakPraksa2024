using DOMAIN.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DOMAIN.Models
{
    public class Reservation
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime timeOfOrder { get; set; }
        public Customer customer { get; set; }
        public decimal price {  get; set; }
        public ReservationStatus status { get; set; } = ReservationStatus.Undefined;
    }
}
