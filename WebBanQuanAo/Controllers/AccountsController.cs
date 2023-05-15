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

namespace WebBanQuanAo.Controllers
{
    [Authorize]
    
    public class AccountsController : Controller
    {
        private readonly db_ClothingContext _context;
        public INotyfService _notyfService { get; }
        public AccountsController(db_ClothingContext context, INotyfService notyfService)
        {
            _context = context;
            _notyfService = notyfService;
        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ValidatePhone(string Phone)
        {
            try
            {
                var khachhang = _context.Accounts.AsNoTracking().SingleOrDefault(x => x.Phone.ToLower() == Phone.ToLower());
                if (khachhang != null)
                    return Json(data: "Số điện thoại : " + Phone + "đã được sử dụng");

                return Json(data: false);

            }
            catch
            {
                return Json(data: true);
            }
        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ValidateEmail(string Email)
        {
            try
            {
                var khachhang = _context.Accounts.AsNoTracking().SingleOrDefault(x => x.Email.ToLower() == Email.ToLower());
                if (khachhang != null)
                    return Json(data: "Email : " + Email + " đã được sử dụng");
                return Json(data: true);
            }
            catch
            {
                return Json(data: false);
            }
        }
        //[Authorize(Roles = "Admin")]
        [AllowAnonymous]
        [Route("tai-khoan-cua-toi.html", Name = "Dashboard")]
        public IActionResult Dashboard()
        {
            var taikhoanID = HttpContext.Session.GetString("CustomerId");
            if (taikhoanID != null)
            {
                var khachhang = _context.Accounts.AsNoTracking().SingleOrDefault(x => x.AccountId == Convert.ToInt32(taikhoanID));
                if (khachhang != null)
                {
                    return View(khachhang);
                }
            }
            return RedirectToAction("Login");
        }
       
        [HttpGet]
        [AllowAnonymous]
        [Route("dang-ky.html", Name = "DangKy")]
        public IActionResult DangkyTaiKhoan()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("dang-ky.html", Name = "DangKy")]
        public async Task<IActionResult> DangkyTaiKhoan(RegisterViewModel taikhoan)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var check = _context.Accounts.FirstOrDefault(s => s.Email == taikhoan.Email);

                    if (check == null)
                    {


                        string salt = Utilities.GetRandomKey();
                        Account khachhang = new Account
                        {
                            FullName = taikhoan.FullName,
                            Phone = taikhoan.Phone.Trim().ToLower(),
                            Email = taikhoan.Email.Trim().ToLower(),
                            Password = (taikhoan.Password + salt.Trim()).ToMD5(),
                            Active = true,
                            Salt = salt,
                            CreateDate = DateTime.Now,
                            RoleId = 3
                           
                        };

                        try
                        {
                            _context.Add(khachhang);
                            await _context.SaveChangesAsync();
                            //Lưu Session MaKh
                            HttpContext.Session.SetString("CustomerId", khachhang.AccountId.ToString());
                            var taikhoanID = HttpContext.Session.GetString("CustomerId");

                            //Identity
                            var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name,khachhang.FullName),
                            new Claim("CustomerId", khachhang.AccountId.ToString())
                        };
                            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "login");
                            ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                            await HttpContext.SignInAsync(claimsPrincipal);
                            _notyfService.Success("Đăng ký thành công");
                            return RedirectToAction("Dashboard", "Accounts");
                        }
                        catch
                        {
                            return RedirectToAction("DangkyTaiKhoan", "Accounts");
                        }
                    }
                    else
                    {
                        ViewBag.error = "Email đã được sử dụng";
                        return View(taikhoan);
                    }
                }
                else
                {
                    return View(taikhoan);
                }
            }
            catch
            {
                return View(taikhoan);
            }
        }
        [Authorize(Roles = "Admin")]
        [HttpGet]
        [AllowAnonymous]
        [Route("dang-nhap.html", Name = "DangNhap")]
        public IActionResult Login(string returnUrl = null)
        {
            var taikhoanID = HttpContext.Session.GetString("CustomerId");
            if (taikhoanID != null)
            {
                return RedirectToAction("Dashboard", "Accounts");
            }
            return View();
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [AllowAnonymous]
        [Route("dang-nhap.html", Name = "DangNhap")]
        public async Task<IActionResult> Login(LoginViewModel customer, string returnUrl)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    bool isEmail = Utilities.IsValidEmail(customer.UserName);
                    if (!isEmail) return View(customer);

                    var khachhang = _context.Accounts.AsNoTracking().SingleOrDefault(x => x.Email.Trim() == customer.UserName);

                    if (khachhang == null) return RedirectToAction("DangkyTaiKhoan");
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
                        new Claim("CustomerId", khachhang.AccountId.ToString())
                    };
                    ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "login");
                    ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                    await HttpContext.SignInAsync(claimsPrincipal);
                    _notyfService.Success("Đăng nhập thành công");
                    if (string.IsNullOrEmpty(returnUrl))
                    {
                        return RedirectToAction("Dashboard", "Accounts");
                    }
                    else
                    {
                        return Redirect(returnUrl);
                    }
                }
            }
            catch
            {
                return RedirectToAction("DangkyTaiKhoan", "Accounts");
            }
            return View(customer);
        }


        [HttpGet]
        [Route("dang-xuat.html", Name = "DangXuat")]
        public IActionResult Logout()
        {
            HttpContext.SignOutAsync();
            HttpContext.Session.Remove("CustomerId");
            _notyfService.Success("Đăng xuất thành công");
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public IActionResult ChangePassword(ChangePasswordViewModel model)
        {
            try
            {
                var taikhoanID = HttpContext.Session.GetString("CustomerId");
                if (taikhoanID == null)
                {
                    return RedirectToAction("Login", "Accounts");
                }
                if (ModelState.IsValid)
                {
                    var taikhoan = _context.Accounts.Find(Convert.ToInt32(taikhoanID));
                    if (taikhoan == null) return RedirectToAction("Login", "Accounts");
                    var pass = (model.PasswordNow.Trim() + taikhoan.Salt.Trim()).ToMD5();
                    {
                        string passnew = (model.Password.Trim() + taikhoan.Salt.Trim()).ToMD5();
                        taikhoan.Password = passnew;
                        _context.Update(taikhoan);
                        _context.SaveChanges();
                        _notyfService.Success("Đổi mật khẩu thành công");
                        return RedirectToAction("Dashboard", "Accounts");
                    }
                }
            }
            catch
            {
                _notyfService.Success("Thay đổi mật khẩu không thành công");
                return RedirectToAction("Dashboard", "Accounts");
            }
            _notyfService.Success("Thay đổi mật khẩu không thành công");
            return RedirectToAction("Dashboard", "Accounts");
        }


    }
}
