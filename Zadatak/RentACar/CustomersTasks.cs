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
                List<dynamic> foundReservation = reservations.Where(x => int.Parse(x.KupacId) == customers[i].id).ToList();
                if (foundReservation.Count != 0)
                {
                     customerTasks.Add(GetWork(customers[i], foundReservation, vehicleService));
                }
                    
            }
            return customerTasks;
        }

        private static Task GetWork(Customer customers, List<dynamic> reservations, VehicleService vehicleService)
        {
            Random rnd = new Random();
            return Task.Run(async () =>
            {
                await Task.Delay(rnd.Next(0, 2) * 1000);
                for(int i=0;i< reservations.Count;i++)
                {
                    Reservation currentUserReservation = new Reservation()
                    {
                        customer = customers,
                        EndDate = DateTime.Parse(reservations[i].KrajRezervacije),
                        StartDate = DateTime.Parse(reservations[i].PocetakRezervacije),
                        timeOfOrder = DateTime.Parse(reservations[i].DatumDolaska)
                    };
                    vehicleService.RentVehicle(currentUserReservation, int.Parse(reservations[i].VoziloId));
                }
            });
        }
    }
}
