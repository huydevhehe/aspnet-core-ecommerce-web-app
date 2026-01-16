using System;
using System.Collections.Generic;

namespace lab1.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string FullName { get; set; }  // Thêm FullName
        public string Address { get; set; }   // Thêm Address
        public string PhoneNumber { get; set; } // Thêm PhoneNumber
        public DateTime OrderDate { get; set; }
        public decimal TotalPrice { get; set; }
        public string Status { get; set; }
        public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
