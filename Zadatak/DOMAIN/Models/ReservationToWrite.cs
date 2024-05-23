using CsvHelper.Configuration.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DOMAIN.Models
{
    public class ReservationToWrite
    {
        public int VoziloId { get; set; }
        public int KupacId { get; set; }

        [Format("dd/MM/yyyy")]
        public DateTime PocetakRezervacije { get; set; }
        [Format("dd/MM/yyyy")]
        public DateTime KrajRezervacije { get; set; }

        public static ReservationToWrite ConvertReservation(Reservation reservation, int vehicleID)
        {
            return new ReservationToWrite()
            {
                VoziloId = vehicleID,
                KupacId = reservation.customer.id,
                PocetakRezervacije = reservation.StartDate,
                KrajRezervacije = reservation.EndDate
            };
        }
    }
}
