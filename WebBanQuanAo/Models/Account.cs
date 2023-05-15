using System;
using System.Collections.Generic;

#nullable disable

namespace WebBanQuanAo.Models
{
    public partial class Account
    {
        public Account()
        {
            Orders = new HashSet<Order>();
        }

        public int AccountId { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string Address { get; set; }
        public bool? Active { get; set; }
        public string Salt { get; set; }
        public string Password { get; set; }
        public int? RoleId { get; set; }
        public DateTime? CreateDate { get; set; }

        public virtual Role Role { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
    }
}
