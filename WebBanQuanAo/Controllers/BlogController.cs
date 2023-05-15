using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PagedList.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebBanQuanAo.Models;

namespace WebBanQuanAo.Controllers
{
    public class BlogController : Controller
    {
        private readonly db_ClothingContext _context;
        public BlogController(db_ClothingContext context)
        {
            _context = context;
        }
        // GET: /<controller>/
        [Route("blogs.html", Name = ("Page"))]
        public IActionResult Index(int? page)
        {
            var pageNumber = page == null || page <= 0 ? 1 : page.Value;
            var pageSize = 2;
            var lsTinDangs = _context.Pages
                .AsNoTracking()
                .OrderByDescending(x => x.PageId);
            PagedList<Page> models = new PagedList<Page>(lsTinDangs, pageNumber, pageSize);

            ViewBag.CurrentPage = pageNumber;
            return View(models);
        }

        [Route("/tin-tuc/{Title}-{id}.html", Name = "PageDetails")]
        public IActionResult Details(int id)
        {
            try
            {
                var tindang = _context.Pages.FirstOrDefault(x => x.PageId == id);

                if (tindang == null)
                {
                    return RedirectToAction("Index");
                }
                return View(tindang);
            }
            catch
            {
                return RedirectToAction("Index", "Home");
            }
        }

    }
}
