using System;
using System.Collections.Generic;

#nullable disable

namespace WebBanQuanAo.Models
{
    public partial class OrderProduct
    {
        public OrderProduct()
        {
            OrderDetails = new HashSet<OrderDetail>();
        }

        public int OrderId { get; set; }
        public int? AccountId { get; set; }
        public int? TranSactStatusId { get; set; }
        public DateTime? OrderDate { get; set; }

        public virtual TransactStatus TranSactStatus { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
