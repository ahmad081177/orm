using System;
using System.Collections.Generic;

namespace Models
{
    public class Order
    {
        public int Id { get; set; }
        public DateTime OrderDateTime { get; set; }
        public Customer Customer { get; set; }
        public List<Product> Products { get; set; }
        public Order() { }
        public Order(int id, DateTime orderDateTime, Customer customer)
        {
            this.Id = id;
            this.Customer = customer;
            this.OrderDateTime = orderDateTime;
            this.Products = new List<Product>();
        }
    }

}