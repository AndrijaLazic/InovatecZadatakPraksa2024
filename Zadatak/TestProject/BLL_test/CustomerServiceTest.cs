﻿using BLL.Services;
using DOMAIN.Abstraction;
using DOMAIN.Models;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestProject.DAL_test;

namespace TestProject.BLL_test
{
    public class CustomerServiceTest
    {
        private CustomerService _customerService;

        public CustomerServiceTest()
        {
            CSVModuleTest cSVModuleTest = new CSVModuleTest();
            _customerService = new CustomerService(cSVModuleTest.csvModule, "kupci.csv", "zahtevi_za_rezervacije.csv", "rezervacije.csv");
        }

        [Fact]
        public void GetCustomers_returnsListOfCustomers()
        {
            List<Customer> customers = _customerService.GetCustomers();
            customers.Should().HaveCount(9);
        }

        [Fact]
        public void GetCustomersReservations_returnsListOfCustomers()
        {
            List<dynamic> reservations = _customerService.GetNewCustomersReservations();
            reservations.Should().HaveCount(10);
        }
    }
}
