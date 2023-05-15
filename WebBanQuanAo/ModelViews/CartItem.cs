using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebBanQuanAo.ModelViews
{
    public class CartItem
    {
        [Key]
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public string Thumb { get; set; }
        public int Price { get; set; }
        public int SoLuong { get; set; }

       
        [Required(ErrorMessage = "Vui lòng chọn kích cỡ")]
        [Display(Name = "Size")]
        public int SizeID { get; set; }
        public string SizeName { get; set; }
        public double ThanhTien => SoLuong * Price;
    }
}
