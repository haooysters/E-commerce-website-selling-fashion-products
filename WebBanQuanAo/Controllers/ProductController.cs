using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PagedList.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebBanQuanAo.Models;

namespace WebBanQuanAo.Controllers
{
    public class ProductController : Controller
    {
        private readonly db_ClothingContext _context;

        public ProductController(db_ClothingContext context)
        {
            _context = context;
        }
        [Route("shop.html", Name = "Product")]
        public IActionResult Index(int page = 1, int CatID = 0)
        {
            var pageNumber = page;
            var pageSize = 6;
            List<Product> lsCourses = new List<Product>();
            if (CatID != 0)
            {
                lsCourses = _context.Products
                .AsNoTracking()
                .Where(x => x.CatId == CatID)
                //.Include(p => p.Brand)
                .Include(p => p.Cat)
                //.Include(p => p.Gender)
                //.Include(p => p.Size)
                .OrderByDescending(x => x.ProductId).ToList();
            }
            else
            {
                lsCourses = _context.Products
                .AsNoTracking()
                //.Include(p => p.Brand)
                .Include(p => p.Cat)
                //.Include(p => p.Gender)
                //.Include(p => p.Size)
                .OrderByDescending(x => x.ProductId).ToList();
            }

            PagedList<Product> models = new PagedList<Product>(lsCourses.AsQueryable(), pageNumber, pageSize);

            ViewBag.CurrentCateID = CatID;
            ViewBag.CurrentPage = pageNumber;

            ViewData["DanhMuc"] = new SelectList(_context.Categories, "CatId", "CatName");
            ViewData["ThuongHieu"] = new SelectList(_context.Brands, "BrandId", "BrandName");
            ViewData["KichCo"] = new SelectList(_context.Sizes, "SizeId", "SizeName", CatID);
            ViewData["GioiTinh"] = new SelectList(_context.Genders, "GenderId", "GenderName", CatID);
            return View(models);
        }

        public IActionResult Filtter(int CatID = 0)

        {
            var url = $"/Product?CatID={CatID}";
            if (CatID == 0)
            {
                url = $"/Product";
            }
            return Json(new { status = "success", redirectUrl = url });
        }

        [Route("/{ProductName}-{id}.html", Name = "ProductDetails")]
        public IActionResult Details(int id)
        {
            try
            {
                var product = _context.Products
                .Include(p => p.Brand)
                .Include(p => p.Cat)
                .Include(p => p.Gender)
                //.Include(p => p.Size)
                .FirstOrDefault(x => x.ProductId == id);
                
                if (product == null)
                {
                    return RedirectToAction("Index");
                }
                ViewData["Size"] = new SelectList(_context.Sizes, "SizeId", "SizeName");
                return View(product);
            }
            catch
            {
                return RedirectToAction("Index", "Home");
            }
        }
    }
}
