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
        private SortedDictionary<DateTime, List<Reservation>> reservations = new SortedDictionary<DateTime, List<Reservation>>();
        private List<Reservation> oldReservations = new List<Reservation>();
        public void addReservation(Reservation reservation)
        {
            if(reservations.TryGetValue(reservation.timeOfOrder,out List<Reservation> currentReservations))
            {
                switch (reservation.customer.membershipType)
                {
                    case MembershipType.VIP:
                        for (int i = 0; i < currentReservations.Count; i++)
                        {
                            if (currentReservations[i].customer.membershipType == MembershipType.VIP)
                                continue;
                            currentReservations.Insert(i, reservation);
                            return;
                        }
                        currentReservations.Add(reservation);
                        return;
                    case MembershipType.Basic:
                        for (int i = 0; i < currentReservations.Count; i++)
                        {
                            if (currentReservations[i].customer.membershipType == MembershipType.VIP || currentReservations[i].customer.membershipType == MembershipType.Basic)
                                continue;
                            currentReservations.Insert(i, reservation);
                            return;
                        }
                        currentReservations.Add(reservation);
                        return;
                    default:
                        currentReservations.Add(reservation);
                        return;
                }
            }
            reservations.Add(reservation.timeOfOrder, new List<Reservation>() { reservation });
        }


        /// <summary>
        /// All valid reservations get appointed and valid/invalid reservations get returned
        /// </summary>
        public void getReservations(out List<Reservation> validNewReservations, out List<Reservation> invalidNewReservations)
        {
            validNewReservations = new List<Reservation>();
            invalidNewReservations= new List<Reservation>();
            List<DateTime> dates = reservations.Keys.ToList();

            //itrerate trough all dates
            for (int i = 0; i < dates.Count; i++)
            {
                 
                //iterate trough all reservations for currentDate. Reservations are sorted based on customers membership
                reservations.TryGetValue(dates[i], out List<Reservation>? reservationsForCurrentDate);

                for (int j = 0; j < reservationsForCurrentDate!.Count; j++)
                {


                    bool isValid = true;
                    for (int z = 0; z < oldReservations.Count; z++)
                    {
                        if (isDateInRange(reservationsForCurrentDate[j].StartDate, oldReservations[z].StartDate, oldReservations[z].EndDate))
                        {
                            isValid = false;
                            reservationsForCurrentDate[j].status = ReservationStatus.AppointmentAlreadyTaken;
                            break;
                        }
                        else if (isDateInRange(reservationsForCurrentDate[j].EndDate, oldReservations[z].StartDate, oldReservations[z].EndDate))
                        {
                            isValid = false;
                            reservationsForCurrentDate[j].status = ReservationStatus.AppointmentAlreadyTaken;
                            break;
                        }
                    }

                    if (!isValid)
                    {
                        invalidNewReservations.Add(reservationsForCurrentDate[j]);
                        continue;
                    }

                    for (int z = 0; z < validNewReservations.Count; z++)
                    {

                        if (isDateInRange(reservationsForCurrentDate[j].StartDate, validNewReservations[z].StartDate, validNewReservations[z].EndDate))
                        {
                            isValid = false;
                            reservationsForCurrentDate[j].status = ReservationStatus.AppointmentAlreadyTaken;
                            break;
                        }
                        else if (isDateInRange(reservationsForCurrentDate[j].EndDate, validNewReservations[z].StartDate, validNewReservations[z].EndDate))
                        {
                            isValid = false;
                            reservationsForCurrentDate[j].status = ReservationStatus.AppointmentAlreadyTaken;
                            break;
                        }
                    }

                    if (!isValid)
                    {
                        invalidNewReservations.Add(reservationsForCurrentDate[j]);
                        continue;
                    }
                    dynamic cashAssetsAftherTransaction = reservationsForCurrentDate[j].customer.cashAssets - reservationsForCurrentDate[j].price;

                    if (cashAssetsAftherTransaction < 0)
                    {
                        reservationsForCurrentDate[j].status = ReservationStatus.NotEnoughCashAssets;
                        invalidNewReservations.Add(reservationsForCurrentDate[j]);
                        continue;
                    }

                    reservationsForCurrentDate[j].status = ReservationStatus.Success;
                    reservationsForCurrentDate[j].customer.cashAssets = cashAssetsAftherTransaction;
                    validNewReservations.Add(reservationsForCurrentDate[j]);
                }
            }
        }

        private bool isDateInRange(DateTime date, DateTime range1, DateTime range2)
        {
            return date >= range1 && date <= range2;
        }

        public void AddOldReservation(Reservation reservations)
        {
            oldReservations.Add(reservations);
        }

        public List<Reservation> GetOldReservations()
        {
            return oldReservations;
        }
    }
}
