using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebBanQuanAo.Extension;
using WebBanQuanAo.Models;
using WebBanQuanAo.ModelViews;

namespace WebBanQuanAo.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly db_ClothingContext _context;
        public INotyfService _notyfService { get; }
        public CheckoutController(db_ClothingContext context, INotyfService notyfService)
        {
            _context = context;
            _notyfService = notyfService;
        }
        public List<CartItem> GioHang
        {
            get
            {
                var gh = HttpContext.Session.Get<List<CartItem>>("GioHang");
                if (gh == default(List<CartItem>))
                {
                    gh = new List<CartItem>();
                }
                return gh;
            }
        }
        [Route("checkout.html", Name = "Checkout")]
        public IActionResult Index(string returnUrl = null)
        {
            //Lay gio hang ra de xu ly
            var cart = HttpContext.Session.Get<List<CartItem>>("GioHang");
            var taikhoanID = HttpContext.Session.GetString("CustomerId");
            MuaHangVM model = new MuaHangVM();
            if (taikhoanID != null)
            {
                var khachhang = _context.Accounts.AsNoTracking().SingleOrDefault(x => x.AccountId == Convert.ToInt32(taikhoanID));
                model.CustomerId = khachhang.AccountId;
                model.FullName = khachhang.FullName;
                model.Email = khachhang.Email;
                model.Phone = khachhang.Phone;
                model.Address = khachhang.Address;
            }
            ViewBag.GioHang = cart;
            return View(model);
        }

        [HttpPost]
        [Route("checkout.html", Name = "Checkout")]
        public IActionResult Index(MuaHangVM muaHang)
        {
            //Lay ra gio hang de xu ly
            var cart = HttpContext.Session.Get<List<CartItem>>("GioHang");
            var taikhoanID = HttpContext.Session.GetString("CustomerId");
            MuaHangVM model = new MuaHangVM();
            if (taikhoanID != null)
            {
                var khachhang = _context.Accounts.AsNoTracking().SingleOrDefault(x => x.AccountId == Convert.ToInt32(taikhoanID));
                model.CustomerId = khachhang.AccountId;
                model.FullName = khachhang.FullName;
                model.Email = khachhang.Email;
                model.Phone = khachhang.Phone;
                model.Address = khachhang.Address;


                khachhang.Phone = muaHang.Phone;
                khachhang.Address = muaHang.Address;
                _context.Update(khachhang);
                _context.SaveChanges();
            }
            try
            {
                if (ModelState.IsValid)
                {


                    //khoi tao don hang
                    Order donhang = new Order();
                    donhang.AccountId = model.CustomerId;
                    donhang.Paid = false;
                    donhang.CreditCard = muaHang.CreditCard;
                    donhang.TransactStatusId = 1;
                    donhang.CreateDate = DateTime.Now;
                    donhang.TotalMoney = Convert.ToInt32(cart.Sum(x => x.ThanhTien));

                    _context.Add(donhang);
                    _context.SaveChanges();
                    //tao danh sach don hang

                    foreach (var item in cart)
                    {
                        OrderDetail orderDetail = new OrderDetail();
                        orderDetail.OrderId = donhang.OrderId;
                        orderDetail.ProductId = item.ProductID;
                        orderDetail.SizeId = item.SizeID;
                        orderDetail.Quantity = item.SoLuong;
                        orderDetail.Total = item.SoLuong * item.Price;

                        _context.Add(orderDetail);
                        //var hangHoa = _context.Products.SingleOrDefault(p => p.ProductId == orderDetail.ProductId);
                        //hangHoa.Amount -= item.SoLuong;
                        //_context.Update(hangHoa);
                        _context.SaveChanges();
                    }
                    _context.SaveChanges();
                  
                    //clear gio hang
                    HttpContext.Session.Remove("GioHang");
                    //Xuat thong bao
                    _notyfService.Success("Đơn hàng đặt thành công");
                    //cap nhat thong tin khach hang
                    return RedirectToAction("Success");


                }
            }
            catch
            {
                ViewBag.GioHang = cart;
                return View(model);
            }
            ViewBag.GioHang = cart;
            return View(model);
        }
        [Route("dat-hang-thanh-cong.html", Name = "Success")]
        public IActionResult Success()
        {
            try
            {
                var taikhoanID = HttpContext.Session.GetString("CustomerId");
                if (string.IsNullOrEmpty(taikhoanID))
                {
                    return RedirectToAction("Login", "Accounts", new { returnUrl = "/dat-hang-thanh-cong.html" });
                }
                var khachhang = _context.Accounts.AsNoTracking().SingleOrDefault(x => x.AccountId == Convert.ToInt32(taikhoanID));
                var donhang = _context.Orders
                    .Where(x => x.AccountId == Convert.ToInt32(taikhoanID))
                    .OrderByDescending(x => x.CreateDate)
                    .FirstOrDefault();
                MuaHangSuccessVM successVM = new MuaHangSuccessVM();
                successVM.FullName = khachhang.FullName;
                successVM.DonHangID = donhang.OrderId;
                successVM.Phone = khachhang.Phone;
                successVM.Address = khachhang.Address;


                return View(successVM);
            }
            catch
            {
                return View();
            }
        }

    }
}
