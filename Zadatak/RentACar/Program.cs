using AutoPlac;
using BLL.Services;
using CsvHelper.Configuration;
using DAL;
using DOMAIN.Abstraction;
using DOMAIN.Enums;
using DOMAIN.Models;
using Serilog;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text.Json;
using System.Text.RegularExpressions;


string text = File.ReadAllText(@"./AppConfig.json");
AppConfig appConfig = JsonSerializer.Deserialize<AppConfig>(text)!;
CsvConfiguration csvConfiguration = new CsvConfiguration(CultureInfo.InvariantCulture)
{
    Delimiter = appConfig.csvConfig.Delimiter
};
CSVModule module = new CSVModule(appConfig, csvConfiguration);
VehicleFactory vehicleFactory = new VehicleFactory();
CustomerService customerService = new CustomerService(module, appConfig);
VehicleService vehicleService = new VehicleService(module, vehicleFactory, appConfig, customerService);

List<Customer> customers = customerService.GetCustomers();

ILogger logger = new LoggerConfiguration()
            .WriteTo.Console(outputTemplate:
            "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
            .CreateLogger();


List<dynamic> newReservations = vehicleService.GetNewCustomersReservations();




//log all customer and vehicle data
for (int i = 0; i < customers.Count; i++)
{
    List<dynamic> foundReservation = newReservations.Where(x => int.Parse(x.KupacId) == customers[i].id).ToList();
    if (foundReservation.Count == 0)
    {
        logger.Information(customers[i].ToString());
        continue;
    }

    string finnalMessage = "\n\n" + customers[i].ToString() + " , i wish to buy vehicle: \n";
    for (int j = 0; j < foundReservation.Count; j++)
    {
        IVehicle wantedVehicle = vehicleService.GetVehicle(int.Parse(foundReservation[j].VoziloId));
        finnalMessage = finnalMessage + wantedVehicle.ToString() + "\n in period of: " + foundReservation[j].PocetakRezervacije + " - " + foundReservation[j].KrajRezervacije + "\n\n";
    }

    logger.Information(finnalMessage);
}




//simulation reservation process
logger.Information("\n\n\nStarting simulation:\n\n");
try
{
    List<Task> customerTasks = CustomersTasks.GetCustomersTasks(customers, newReservations, vehicleService);
    Task.WaitAll(customerTasks.ToArray());
}
catch (AggregateException ex)
{
    //https://stackoverflow.com/questions/12007781/why-doesnt-await-on-task-whenall-throw-an-aggregateexception
    if (ex.InnerExceptions.Count == 0)
        throw;
    foreach (AggregateException taskExs in ex.InnerExceptions)
    {
        foreach (var singleEx in taskExs.InnerExceptions)
            logger.Error(singleEx.Message);
    }
}
catch (Exception ex)
{
    logger.Error(ex.Message);
}



List<ReservationToWrite> successfullReservations = new List<ReservationToWrite>();

List<IVehicle> vehicles = vehicleService.GetVehicles();

for (int i = 0; i < vehicles.Count; i++)
{
    if (vehicles[i].id == 4)
    {

    }
    vehicles[i].reservation.getReservations(out List<Reservation> validReservations, out List<Reservation> invalidReservations);
    for (int j = 0; j < validReservations.Count; j++)
    {
        successfullReservations.Add(ReservationToWrite.ConvertReservation(validReservations[j], vehicles[i].id));
    }
    for (int j = 0;j < invalidReservations.Count; j++)
    {
        if (invalidReservations[j].status == ReservationStatus.AppointmentAlreadyTaken)
            logger.Error("User: " + invalidReservations[j].customer.name + " " + invalidReservations[j].customer.lastName + 
                " failed to reserve the vehicle:" + vehicles[i].id +" because it has already been reserved for the given period.");
        else if(invalidReservations[j].status == ReservationStatus.NotEnoughCashAssets)
            logger.Error("User: " + invalidReservations[j].customer.name + " " + invalidReservations[j].customer.lastName + 
                " failed to reserve the vehicle:" + vehicles[i].id + " because there is not enough cash assets in customers account.("
                + invalidReservations[j].customer.cashAssets+"/"+invalidReservations[j].price+ ")");
    }
}






module.WriteFile("nove_rezervacije.csv", successfullReservations);
/*module.WriteFile("kupci.csv", customers);
*/

logger.Information("\n\n\nEnded simulation:\n\n");

