using System;
using System.Collections.Generic;
using Dsw2025Tpi.Domain.Entities;

namespace Dsw2025Tpi.Domain.Entities
{
    public class Customer : EntityBase
    {
        public Customer()
        {
            Orders = new List<Order>();
        }

        public Customer(string name, string email, string phoneNumber)
        {
            Name = name;
            Email = email;
            PhoneNumber = phoneNumber;
            Orders = new List<Order>();
        }

        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;

        public ICollection<Order> Orders { get; set; }
    }
}