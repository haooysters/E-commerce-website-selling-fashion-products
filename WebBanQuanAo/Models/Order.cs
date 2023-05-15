using System;
using System.Collections.Generic;

#nullable disable

namespace WebBanQuanAo.Models
{
    public partial class Order
    {
        public Order()
        {
            OrderDetails = new HashSet<OrderDetail>();
        }

        public int OrderId { get; set; }
        public int? AccountId { get; set; }
        public int? TransactStatusId { get; set; }
        public bool? Paid { get; set; }
        public int? CreditCard { get; set; }
        public int? TotalMoney { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? ShipDate { get; set; }

        public virtual Account Account { get; set; }
        public virtual TransactStatus TransactStatus { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
