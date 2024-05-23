using BLL.Services;
using DOMAIN.Abstraction;
using DOMAIN.Models;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoPlac
{
    public class CustomersTasks
    {
        public static List<Task> GetCustomersTasks(List<Customer> customers, List<dynamic> reservations,VehicleService vehicleService)
        {
            List<Task> customerTasks = new List<Task>();
            

            for (int i = 0; i < customers.Count; i++)
            {
                dynamic? foundReservation = reservations.FirstOrDefault(x => int.Parse(x.KupacId) == customers[i].id);
                if(foundReservation!=null)
                    customerTasks.Add(GetWork(customers[i], foundReservation,  vehicleService));
            }
            return customerTasks;
        }

        private static Task GetWork(Customer customers, dynamic reservation, VehicleService vehicleService)
        {
            Random rnd = new Random();
            return Task.Run(async () =>
            {
                await Task.Delay(rnd.Next(0, 2) * 1000);
                Reservation currentUserReservation = new Reservation()
                {
                    customer = customers,
                    EndDate = DateTime.Parse(reservation.KrajRezervacije),
                    StartDate = DateTime.Parse(reservation.PocetakRezervacije),
                    timeOfOrder = DateTime.Parse(reservation.DatumDolaska)
                };
                if(customers.id == 9)
                {

                }
                vehicleService.RentVehicle(currentUserReservation, int.Parse(reservation.VoziloId));
            });
        }
    }
}
