using System;
using System.Collections.Generic;

#nullable disable

namespace WebBanQuanAo.Models
{
    public partial class Size
    {
        public Size()
        {
            OrderDetails = new HashSet<OrderDetail>();
        }

        public int SizeId { get; set; }
        public string SizeName { get; set; }
        public string Description { get; set; }

        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
