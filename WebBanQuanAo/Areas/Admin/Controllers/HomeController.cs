using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebBanQuanAo.Areas.Admin.Controllers
{
    [Area("Admin")]
    //[Route("login.html", Name = "Login")]
    [Authorize(Roles = "Admin, Staff")]
    public class HomeController : Controller
    {
        
        
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
