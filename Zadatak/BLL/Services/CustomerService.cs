using DAL;
using DOMAIN.Abstraction;
using DOMAIN.Models;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class CustomerService : ICustomerService
    {
        private CSVModule _csvModule;
        private Dictionary<int,Customer> _customers;
        private string _fileNameCustomers;
        private string _fileNameNewCustomersReservations;
        private string _fileNameOldCustomersReservations;

        public CustomerService(CSVModule csvModule, string fileNameCustomers, string fileNameNewCustomersReservations, string fileNameOldCustomersReservations)
        {
            _csvModule = csvModule;
            _fileNameCustomers = fileNameCustomers;
            _fileNameNewCustomersReservations = fileNameNewCustomersReservations;
            _fileNameOldCustomersReservations = fileNameOldCustomersReservations;
            _customers= new Dictionary<int,Customer>();
            LoadCustomers();
        }

        private void LoadCustomers()
        {
            List<dynamic> readCustomers = _csvModule.ReadFile(_fileNameCustomers);
    
            for(int i = 0; i < readCustomers.Count; i++)
            {
                Customer newCustomer = Customer.GetCustomer(readCustomers[i]);
                _customers.Add(newCustomer.id, newCustomer);
            }

        }

        public List<Customer> GetCustomers()
        {
            return _customers.Values.ToList();
        }

        public List<dynamic> GetNewCustomersReservations() {
            List<dynamic> reservations = _csvModule.ReadFile(_fileNameNewCustomersReservations);
            for(int i = 0; i < reservations.Count; i++)
            {
                dynamic expando = new ExpandoObject();
                expando.VoziloId = reservations[i].VoziloId;
                expando.KupacId= reservations[i].KupacId;
                expando.DatumDolaska = DateTime.Parse(reservations[i].DatumDolaska).ToString("dd/MM/yyyy");
                expando.PocetakRezervacije = DateTime.Parse(reservations[i].PocetakRezervacije).ToString("dd/MM/yyyy");
                DateTime finnishDate= DateTime.Parse(reservations[i].PocetakRezervacije);
                finnishDate = finnishDate.AddDays(int.Parse(reservations[i].BrojDana));
                expando.KrajRezervacije= finnishDate.ToString("dd/MM/yyyy");
                reservations[i]= expando;
            }
            return  reservations;
        }

        public List<dynamic> GetOldCustomersReservations()
        {
            return _csvModule.ReadFile(_fileNameOldCustomersReservations);
        }
    }
}
