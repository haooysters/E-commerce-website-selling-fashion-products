using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using WebBanQuanAo.Extension;
using WebBanQuanAo.Helpper;
using WebBanQuanAo.Models;
using WebBanQuanAo.ModelViews;

namespace WebBanQuanAo.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]

    public class LoginController : Controller
    {
        private readonly db_ClothingContext _context;

        public INotyfService _notyfService { get; }

        public LoginController(db_ClothingContext context, INotyfService notyfService)
        {
            _context = context;
            _notyfService = notyfService;
        }

        //[Area("Admin")]
        [HttpGet]
        [AllowAnonymous]
        //[Route("login.html", Name = "Login")]
        //[Route("admin-dang-nhap.html", Name = "AdminDangNhap")]
        public IActionResult Login(string returnUrl = null)
        {
            var taikhoanID = HttpContext.Session.GetString("CustomerId");
            if (taikhoanID != null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }
        //[Area("Admin")]
        [HttpPost]
        [AllowAnonymous]
        //[Route("login.html", Name = "Login")]
        //[Route("admin-dang-nhap.html", Name = "AdminDangNhap")]
        public async Task<IActionResult> Login(LoginViewModel customer, string returnUrl)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    bool isEmail = Utilities.IsValidEmail(customer.UserName);
                    if (!isEmail) return View(customer);

                    var khachhang = _context.Accounts.Include(p => p.Role).AsNoTracking().SingleOrDefault(x => x.Email.Trim() == customer.UserName);

                    if (khachhang == null)
                    {
                        _notyfService.Error("Thông tin đăng nhập chưa chính xác");
                        return View(customer);
                    }
                       
                    string pass = (customer.Password + khachhang.Salt.Trim()).ToMD5();
                    if (khachhang.Password != pass)
                    {
                        _notyfService.Error("Thông tin đăng nhập chưa chính xác");
                        return View(customer);
                    }
                    //kiem tra xem account co bi disable hay khong

                    if (khachhang.Active == false)
                    {
                        _notyfService.Error("Tài khoản của bạn đã bị khóa");
                        return View(customer);
                    }

                    //Luu Session MaKh
                    HttpContext.Session.SetString("CustomerId", khachhang.AccountId.ToString());
                    var taikhoanID = HttpContext.Session.GetString("CustomerId");

                    //Identity
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, khachhang.FullName),
                        new Claim("CustomerId", khachhang.AccountId.ToString()),                        
                        new Claim("RoleId", khachhang.RoleId.ToString()),
                        new Claim(ClaimTypes.Role, khachhang.Role.RoleName)
                    };
                    ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "login");
                    ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                    await HttpContext.SignInAsync(claimsPrincipal);
                   
                    if (string.IsNullOrEmpty(returnUrl))
                    {
                        _notyfService.Success("Đăng nhập thành công");
                        return RedirectToAction("Index", "Home");
                       
                    }
                    else
                    {
                        return Redirect(returnUrl);
                    }               
                }
            }
            catch
            {
                return View(customer);
            }
            return View(customer);
        }

        [HttpGet]
        //[Route("admin-dang-xuat.html", Name = "DangXuat")]
        public IActionResult Logout()
        {
            HttpContext.SignOutAsync();
            HttpContext.Session.Remove("CustomerId");
            _notyfService.Success("Đăng xuất thành công");
            return RedirectToAction("Login", "Login");
        }

        [HttpGet]
        //[Route("admin-dang-xuat.html", Name = "DangXuat")]
        public IActionResult AccessDenied()
        {
            HttpContext.Session.Remove("CustomerId");
            _notyfService.Error("Không có quyền truy cập!");         
            return View();
        }
    }
}
