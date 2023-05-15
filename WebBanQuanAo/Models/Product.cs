using System;
using System.Collections.Generic;

#nullable disable

namespace WebBanQuanAo.Models
{
    public partial class Product
    {
        public Product()
        {
            OrderDetails = new HashSet<OrderDetail>();
        }

        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ShortDesc { get; set; }
        public string Description { get; set; }
        public int? Amount { get; set; }
        public int? CatId { get; set; }
        public int? BrandId { get; set; }
        public int? GenderId { get; set; }
        public bool? Active { get; set; }
        public int? Price { get; set; }
        public string Thumb { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? ModifiedDate { get; set; }

        public virtual Brand Brand { get; set; }
        public virtual Category Cat { get; set; }
        public virtual Gender Gender { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
