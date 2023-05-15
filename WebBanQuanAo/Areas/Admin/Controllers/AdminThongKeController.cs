using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebBanQuanAo.Models;

namespace WebBanQuanAo.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin, Staff")]
    public class AdminThongKeController : Controller
    {
        private readonly db_ClothingContext _context;

        public AdminThongKeController(db_ClothingContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            ViewBag.TongDoanhThu = ThongKeDoanhThu();
            ViewBag.TongDonDangKy = ThongKeDonDangKy();
            ViewBag.TongHocVien = ThongKeHocVien();
            var db_ClothingContext = _context.OrderDetails.Include(o => o.Product).Include(o => o.Order);
            return View();
        }
        //Tổng doanh thu từ khi thành lập web
        public Decimal ThongKeDoanhThu()
        {
            decimal TongDoanhThu = _context.OrderDetails.Sum(n => n.Total).Value;

            return TongDoanhThu;
        }

        //Thống kê đơn đăng ký
        public Double ThongKeDonDangKy()
        {
            double ddk = _context.OrderDetails.Count();

            return ddk;
        }

        //Thống kê tổng số học viên 
        public Double ThongKeHocVien()
        {
            double hv = _context.Orders.Count();

            return hv;
        }
    }
}
