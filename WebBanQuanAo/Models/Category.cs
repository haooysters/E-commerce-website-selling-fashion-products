using System;
using System.Collections.Generic;

#nullable disable

namespace WebBanQuanAo.Models
{
    public partial class Category
    {
        public Category()
        {
            Products = new HashSet<Product>();
        }

        public int CatId { get; set; }
        public string CatName { get; set; }
        public string Description { get; set; }

        public virtual ICollection<Product> Products { get; set; }
    }
}
