using DOMAIN.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DOMAIN.Abstraction
{
    public interface ICustomerService
    {
        public List<Customer> GetCustomers();
        public Customer GetCustomerById(int id);
    }
}
