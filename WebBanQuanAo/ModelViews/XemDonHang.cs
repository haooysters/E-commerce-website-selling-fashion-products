using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WebBanQuanAo.Models;

namespace WebBanQuanAo.ModelViews
{
    public class XemDonHang
    {
       
        public Order DonHang { get; set; }
        public List<OrderDetail> ChiTietDonHang { get; set; }
    }
}
