using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebBanQuanAo.Models;

namespace WebBanQuanAo.Controllers
{
    public class SearchController : Controller
    {
        private readonly db_ClothingContext _context;
        public INotyfService _notifyService { get; }
        public SearchController(db_ClothingContext context, INotyfService notifyService)
        {
            _context = context;
            _notifyService = notifyService;
        }

        [Route("search.html", Name = "Search")]
        public IActionResult KQSearch(string sKey)
        {

            // tim kiem theo ten khoa hoc
            var lsTKH = _context.Products.Where(n => n.ProductName.Contains(sKey));

            if (lsTKH.Count() == 0)
            {
                _notifyService.Error("Không tìm thấy sản phẩm nào!");
                return RedirectToAction("Index", "Home");
            }
            return View(lsTKH.OrderBy(n => n.ProductName));
        }
    }
}
