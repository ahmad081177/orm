using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Order
    {
        public int Id { get; set; }
        public DateTime OrderDateTime { get; set; }
        public Customer Customer { get; set; }
        public Order() { }
        public Order(int id, DateTime orderDateTime, Customer customer)
        {
            this.Id = id;
            this.Customer = customer;
            this.OrderDateTime = orderDateTime;
        }
    }

}
