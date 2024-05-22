﻿using DAL;
using DOMAIN.Abstraction;
using DOMAIN.Models;
using System;
using System.Collections.Generic;
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
        private string _fileNameCustomersReservations;

        public CustomerService(CSVModule csvModule, string fileNameCustomers, string fileNameCustomersReservations)
        {
            _csvModule = csvModule;
            _customers = new Dictionary<int,Customer>();
            _fileNameCustomers = fileNameCustomers;
            _fileNameCustomersReservations = fileNameCustomersReservations;
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

        public List<dynamic> GetCustomersReservations() {
            return _csvModule.ReadFile(_fileNameCustomersReservations);
        }
    }
}