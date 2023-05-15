using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PagedList.Core;
using WebBanQuanAo.Helpper;
using WebBanQuanAo.Models;

namespace WebBanQuanAo.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin, Staff")]
    public class AdminProductsController : Controller
    {
        private readonly db_ClothingContext _context;

        public INotyfService _notifyService { get; }

        public AdminProductsController(db_ClothingContext context, INotyfService notifyService)
        {
            _context = context;
            _notifyService = notifyService;
        }

        // GET: Admin/AdminProducts

        public IActionResult Index(int page = 1, int CatID = 0)
        {
            var pageNumber = page;
            var pageSize = 5;
            List<Product> lsCourses = new List<Product>();
            if (CatID != 0)
            {
                lsCourses = _context.Products
                .AsNoTracking()
                .Where(x => x.CatId == CatID)
                .Include(p => p.Brand)
                .Include(p => p.Cat)
                .Include(p => p.Gender)
                //.Include(p => p.Size)
                .OrderByDescending(x => x.ProductId).ToList();
            }
            else
            {
                lsCourses = _context.Products
                .AsNoTracking()
                .Include(p => p.Brand)
                .Include(p => p.Cat)
                .Include(p => p.Gender)
                //.Include(p => p.Size)
                .OrderByDescending(x => x.ProductId).ToList();
            }

            PagedList<Product> models = new PagedList<Product>(lsCourses.AsQueryable(), pageNumber, pageSize);

            ViewBag.CurrentCateID = CatID;
            ViewBag.CurrentPage = pageNumber;

            ViewData["DanhMuc"] = new SelectList(_context.Categories, "CatId", "CatName", CatID);
            ViewData["ThuongHieu"] = new SelectList(_context.Brands, "BrandId", "BrandName");
            ViewData["KichCo"] = new SelectList(_context.Sizes, "SizeId", "SizeName", CatID);
            ViewData["GioiTinh"] = new SelectList(_context.Genders, "GenderId", "GenderName", CatID);
            return View(models);
        }
        public IActionResult Filtter(int CatID = 0)

        {
            var url = $"/Admin/AdminProducts?CatID={CatID}";
            if (CatID == 0)
            {
                url = $"/Admin/AdminProducts";
            }
            return Json(new { status = "success", redirectUrl = url });
        }

        // GET: Admin/AdminProducts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Brand)
                .Include(p => p.Cat)
                .Include(p => p.Gender)
                //.Include(p => p.Size)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Admin/AdminProducts/Create
        public IActionResult Create()
        {
            ViewData["DanhMuc"] = new SelectList(_context.Categories, "CatId", "CatName");
            ViewData["ThuongHieu"] = new SelectList(_context.Brands, "BrandId", "BrandName");
            ViewData["KichCo"] = new SelectList(_context.Sizes, "SizeId", "SizeName");
            ViewData["GioiTinh"] = new SelectList(_context.Genders, "GenderId", "GenderName");
            return View();
        }

        // POST: Admin/AdminProducts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProductId,ProductName,ShortDesc,Description,Amount,CatId,BrandId,SizeId,GenderId,Active,Title,Price,Thumb,CreateDate,ModifiedDate")] Product product, Microsoft.AspNetCore.Http.IFormFile fThumb)
        {
            if (ModelState.IsValid)
            {
                product.ProductName = Utilities.ToTitleCase(product.ProductName);
                if (fThumb != null)
                {
                    string extension = Path.GetExtension(fThumb.FileName);
                    string image = Utilities.SEOUrl(product.ProductName) + extension;
                    product.Thumb = await Utilities.UploadFile(fThumb, @"products", image.ToLower());
                }
                if (string.IsNullOrEmpty(product.Thumb)) product.Thumb = "default.jpg";
                
                product.CreateDate = DateTime.Now;
                

                _context.Add(product);
                await _context.SaveChangesAsync();
                _notifyService.Success("Thêm mới thành công");
                return RedirectToAction(nameof(Index));
            }
            ViewData["DanhMuc"] = new SelectList(_context.Categories, "CatId", "CatName");
            ViewData["ThuongHieu"] = new SelectList(_context.Brands, "BrandId", "BrandName");
            ViewData["KichCo"] = new SelectList(_context.Sizes, "SizeId", "SizeName");
            ViewData["GioiTinh"] = new SelectList(_context.Genders, "GenderId", "GenderName");
            return View(product);
        }

        // GET: Admin/AdminProducts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            ViewData["DanhMuc"] = new SelectList(_context.Categories, "CatId", "CatName");
            ViewData["ThuongHieu"] = new SelectList(_context.Brands, "BrandId", "BrandName");
            ViewData["KichCo"] = new SelectList(_context.Sizes, "SizeId", "SizeName");
            ViewData["GioiTinh"] = new SelectList(_context.Genders, "GenderId", "GenderName");
            return View(product);
        }

        // POST: Admin/AdminProducts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProductId,ProductName,ShortDesc,Description,Amount,CatId,BrandId,SizeId,GenderId,Active,Title,Price,Thumb,CreateDate,ModifiedDate")] Product product, Microsoft.AspNetCore.Http.IFormFile fThumb)
        {
            if (id != product.ProductId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    product.ProductName = Utilities.ToTitleCase(product.ProductName);
                    if (fThumb != null)
                    {
                        string extension = Path.GetExtension(fThumb.FileName);
                        string image = Utilities.SEOUrl(product.ProductName) + extension;
                        product.Thumb = await Utilities.UploadFile(fThumb, @"products", image.ToLower());
                    }
                    if (string.IsNullOrEmpty(product.Thumb)) product.Thumb = "default.jpg";
                    //product.Alias = Utilities.SEOUrl(product.ProductName);
                    product.ModifiedDate = DateTime.Now;

                    _context.Update(product);
                    _notifyService.Success("Cập nhật thành công");
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.ProductId))
                    {
                        _notifyService.Success("Có xảy ra lỗi!");
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["DanhMuc"] = new SelectList(_context.Categories, "CatId", "CatName");
            ViewData["ThuongHieu"] = new SelectList(_context.Brands, "BrandId", "BrandName");
            ViewData["KichCo"] = new SelectList(_context.Sizes, "SizeId", "SizeName");
            ViewData["GioiTinh"] = new SelectList(_context.Genders, "GenderId", "GenderName");
            return View(product);
        }

        // GET: Admin/AdminProducts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Brand)
                .Include(p => p.Cat)
                .Include(p => p.Gender)
                //.Include(p => p.Size)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Admin/AdminProducts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.ProductId == id);
        }
    }
}
