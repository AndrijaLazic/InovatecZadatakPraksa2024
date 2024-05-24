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
    if (ex.InnerExceptions.Count == 0)
        throw;
    foreach (var innerEx in ex.InnerExceptions)
    {
        logger.Error(innerEx.Message);
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
    List<Reservation> allReservations = vehicles[i].reservation.getReservations();
    for (int j = 0; j < allReservations.Count; j++)
    {
        if (allReservations[j].status == ReservationStatus.Success)
        {
            successfullReservations.Add(ReservationToWrite.ConvertReservation(allReservations[j], vehicles[i].id));
            continue;
        }
        if (allReservations[j].status == ReservationStatus.AppointmentAlreadyTaken)
        {
            logger.Error("User: " + allReservations[j].customer.name + " " + allReservations[j].customer.lastName + " failed to reserve the vehicle because it has already been reserved for the given period.");
        }
    }
}






module.WriteFile("nove_rezervacije.csv", successfullReservations);
module.WriteFile("kupci.csv", customers);


logger.Information("\n\n\nEnded simulation:\n\n");

