using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebBanQuanAo.ModelViews
{
    public class MuaHangSuccessVM
    {
     
        public int DonHangID { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
      
    }
}
