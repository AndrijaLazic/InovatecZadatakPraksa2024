using DOMAIN.Abstraction;
using DOMAIN.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DOMAIN.Models
{
    public class VehicleReservation
    {
        private List<Reservation> vipCustomers = new List<Reservation>();
        private List<Reservation> basicCustomers = new List<Reservation>();
        private List<Reservation> normalCustomers = new List<Reservation>();

        public void addReservation(Reservation reservation)
        {
            switch (reservation.customer.membershipType)
            {
                case MembershipType.VIP:
                    for (int i = 0; i < vipCustomers.Count; i++)
                    {
                        if (reservation.timeOfOrder >= vipCustomers[i].timeOfOrder)
                            continue;
                        vipCustomers.Insert(i, reservation);
                        return;
                    }
                    vipCustomers.Add(reservation);
                    break;
                case MembershipType.Basic:
                    for (int i = 0; i < basicCustomers.Count; i++)
                    {
                        if (reservation.timeOfOrder >= basicCustomers[i].timeOfOrder)
                            continue;
                        basicCustomers.Insert(i, reservation);
                        return;
                    }
                    basicCustomers.Add(reservation);
                    break;
                default:
                    for (int i = 0; i < normalCustomers.Count; i++)
                    {
                        if (reservation.timeOfOrder >= normalCustomers[i].timeOfOrder)
                            continue;
                        normalCustomers.Insert(i, reservation);
                        return;
                    }
                    normalCustomers.Add(reservation);
                    break;
            }
        }

        public List<Reservation> getReservations()
        {
            List<Reservation> allReservations=new List<Reservation>();
            for (int i = 0; i< vipCustomers.Count; i++)
            {
                bool isValid = true;
                for (int j = 0; j < allReservations.Count; j++)
                {
                    if (this.isDateInRange(vipCustomers[i].StartDate, allReservations[j].StartDate, allReservations[j].EndDate))
                    {
                        isValid = false;
                        vipCustomers[i].status = ReservationStatus.AppointmentAlreadyTaken;
                        break;
                    }
                    else if (this.isDateInRange(vipCustomers[i].EndDate, allReservations[j].StartDate, allReservations[j].EndDate))
                    {
                        isValid = false;
                        vipCustomers[i].status = ReservationStatus.AppointmentAlreadyTaken;
                        break;
                    }
                }
                if (isValid)
                {
                    vipCustomers[i].status= ReservationStatus.Success;
                    vipCustomers[i].customer.cashAssets = vipCustomers[i].customer.cashAssets - vipCustomers[i].price;
                }
                allReservations.Add(vipCustomers[i]);

            }

            for (int i = 0; i < basicCustomers.Count; i++)
            {
                bool isValid = true;
                for (int j = 0; j < allReservations.Count; j++)
                {
                    if (this.isDateInRange(basicCustomers[i].StartDate, allReservations[j].StartDate, allReservations[j].EndDate))
                    {
                        isValid = false;
                        basicCustomers[i].status = ReservationStatus.AppointmentAlreadyTaken;
                        break;
                    }
                    else if (this.isDateInRange(basicCustomers[i].EndDate, allReservations[j].StartDate, allReservations[j].EndDate))
                    {
                        isValid = false;
                        basicCustomers[i].status = ReservationStatus.AppointmentAlreadyTaken;
                        break;
                    }
                }
                if (isValid)
                {
                    basicCustomers[i].status = ReservationStatus.Success;
                    basicCustomers[i].customer.cashAssets = basicCustomers[i].customer.cashAssets - basicCustomers[i].price;
                }
                allReservations.Add(basicCustomers[i]);

            }

            for (int i = 0; i < normalCustomers.Count; i++)
            {
                bool isValid = true;
                for (int j = 0; j < allReservations.Count; j++)
                {
                    if (this.isDateInRange(normalCustomers[i].StartDate, allReservations[j].StartDate, allReservations[j].EndDate))
                    {
                        isValid = false;
                        normalCustomers[i].status = ReservationStatus.AppointmentAlreadyTaken;
                        break;
                    }
                    else if (this.isDateInRange(normalCustomers[i].EndDate, allReservations[j].StartDate, allReservations[j].EndDate))
                    {
                        isValid = false;
                        normalCustomers[i].status = ReservationStatus.AppointmentAlreadyTaken;
                        break;
                    }
                }
                if (isValid)
                {
                    normalCustomers[i].status = ReservationStatus.Success;
                    normalCustomers[i].customer.cashAssets = normalCustomers[i].customer.cashAssets - normalCustomers[i].price;
                }
                allReservations.Add(normalCustomers[i]);
            }
            return allReservations;
        }

        private bool isDateInRange(DateTime date, DateTime range1, DateTime range2)
        {
            return date >= range1 && date <= range2;
        }
    }
}
