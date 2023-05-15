using System;
using System.Collections.Generic;

#nullable disable

namespace WebBanQuanAo.Models
{
    public partial class Customer
    {
        public Customer()
        {
            OrderDetails = new HashSet<OrderDetail>();
        }

        public int CustomerId { get; set; }
        public string FullName { get; set; }
        public DateTime? Birthday { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public DateTime? CreateDate { get; set; }

        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
