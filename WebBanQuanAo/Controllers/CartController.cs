using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using WebBanQuanAo.Extension;
using WebBanQuanAo.Models;
using WebBanQuanAo.ModelViews;

namespace WebBanQuanAo.Controllers
{
    public class CartController : Controller
    {
        private readonly db_ClothingContext _context;
        public INotyfService _notifyService { get; }

        private readonly string _clientId;
        private readonly string _secretKey;

        public double TyGiaUSD = 23300;//store in Database

        public CartController(db_ClothingContext context, INotyfService notifyService, IConfiguration config)
        {
            _context = context;
            _notifyService = notifyService;
            _clientId = config["PaypalSettings:ClientId"];
            _secretKey = config["PaypalSettings:SecretKey"];
        }

        //Khởi tạo giỏ hàng
        public List<CartItem> Carts
        {
            get
            {

                var data = HttpContext.Session.Get<List<CartItem>>("GioHang");
                if (data == null)
                {
                    data = new List<CartItem>();
                }
                return data;
            }
        }
        [Route("cart.html", Name = "Cart")]
        public IActionResult Index()
        {
            ViewBag.Error = "";
            return View(Carts);
        }

        public IActionResult AddToCart(int id, int idsize, int SoLuong, string type = "Normal")
        {
            var myCart = Carts;
            var item = myCart.SingleOrDefault(p => p.ProductID == id && p.SizeID == idsize);
            //var size = myCart.SingleOrDefault(p => p.SizeID == idsize);


            //Kiểm tra khóa học active ?
            var check = _context.Products
                .Where(s => s.ProductId == id && s.Active == true).FirstOrDefault();
            //Kiểm tra số lượng nhập vào
            var checkSl = _context.Products.AsNoTracking().SingleOrDefault(s => s.ProductId == id && s.Amount >= SoLuong);
            if (check != null)
            {
                if (checkSl != null)
                {
                    var hangHoa = _context.Products.SingleOrDefault(p => p.ProductId == id);
                    var kichco = _context.Sizes.SingleOrDefault(p => p.SizeId == idsize);
                    if (item == null)//chưa có
                    {

                        item = new CartItem
                        {
                            ProductID = id,
                            ProductName = hangHoa.ProductName,
                            Price = hangHoa.Price.Value,
                            SoLuong = SoLuong,
                            Thumb = hangHoa.Thumb,
                            SizeID = idsize,
                            SizeName = kichco.SizeName
                        };
                        myCart.Add(item);

                    }
                    else
                    {

                        item.SoLuong += SoLuong;
                        //hangHoa.Amount -= SoLuong;
                        //_context.Update(hangHoa);
                        //_context.SaveChanges();


                        //item.SoLuong += SoLuong;
                    }
                    HttpContext.Session.Set("GioHang", myCart);
                    _notifyService.Success("Thêm giỏ hàng thành công!");


                    if (type == "ajax")
                    {
                        return Json(new
                        {
                            SoLuong = Carts.Sum(c => c.SoLuong)
                        });
                    }
                }
                else
                {
                    _notifyService.Error("Sản phẩm hết hàng!");
                }
            }
            else
            {
                _notifyService.Error("Sản phẩm hết hàng!");
            }
            return RedirectToAction("Index");
        }

        //Xóa giỏ hàng
        [HttpPost]
        [Route("api/cart/remove")]
        public ActionResult Remove(int productID, int sizeID)
        {

            try
            {
                List<CartItem> gioHang = Carts;
                CartItem item = gioHang.SingleOrDefault(p => p.ProductID == productID && p.SizeID == sizeID);
                if (item != null)
                {
                    gioHang.Remove(item);
                }
                //luu lai session
                HttpContext.Session.Set<List<CartItem>>("GioHang", gioHang);
                _notifyService.Error("Xóa giỏ hàng thành công!");
                return Json(new { success = true });
            }
            catch
            {
                return Json(new { success = false });
            }
        }

        //Cập nhật giỏ hàng

        [HttpPost]
        [Route("api/cart/update")]
        public IActionResult UpdateCart(int productID, int sizeID, int? amount)
        {
            var checkSl = _context.Products.AsNoTracking().SingleOrDefault(s => s.ProductId == productID && s.Amount >= amount);

            //Lay gio hang ra de xu ly
            var cart = HttpContext.Session.Get<List<CartItem>>("GioHang");
            try
            {
                if (checkSl != null)
                {
                    if (cart != null)
                    {
                        CartItem item = cart.SingleOrDefault(p => p.ProductID == productID && p.SizeID == sizeID);
                        if (item != null && amount.HasValue) // da co -> cap nhat so luong
                        {
                            item.SoLuong = amount.Value;
                        }
                        //Luu lai session
                        HttpContext.Session.Set<List<CartItem>>("GioHang", cart);
                    }
                }
                else
            {
                _notifyService.Error("Sản phẩm hết hàng!");
            }
                return Json(new { success = true });
            }
            catch
            {
                return Json(new { success = false });
            }
        }
    }
}
