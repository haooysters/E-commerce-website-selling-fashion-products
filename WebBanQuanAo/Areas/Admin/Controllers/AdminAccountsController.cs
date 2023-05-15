using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PagedList.Core;
using WebBanQuanAo.Models;

namespace WebBanQuanAo.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class AdminAccountsController : Controller
    {
        private readonly db_ClothingContext _context;

        public INotyfService _notifyService { get; }

        public AdminAccountsController(db_ClothingContext context, INotyfService notifyService)
        {
            _context = context;
            _notifyService = notifyService;
        }

        // GET: Admin/AdminAccounts
        //public async Task<IActionResult> Index()
        //{
        //    var db_ClothingContext = _context.Accounts.Include(a => a.Role);

        //    ViewData["Quyen"] = new SelectList(_context.Roles, "RoleId", "RoleName");

        //    return View(await db_ClothingContext.ToListAsync());
        //}

        public IActionResult Index(int page = 1, int CatID = 0)
        {
            var pageNumber = page;
            var pageSize = 5;
            List<Account> lsCourses = new List<Account>();
            if (CatID != 0)
            {
                lsCourses = _context.Accounts
                .AsNoTracking()
                .Where(x => x.RoleId == CatID)
                .Include(x => x.Role)
                .OrderByDescending(x => x.AccountId).ToList();
            }
            else
            {
                lsCourses = _context.Accounts
                .AsNoTracking()
                .Include(x => x.Role)
                .OrderByDescending(x => x.AccountId).ToList();
            }

            PagedList<Account> models = new PagedList<Account>(lsCourses.AsQueryable(), pageNumber, pageSize);

            ViewBag.CurrentCateID = CatID;
            ViewBag.CurrentPage = pageNumber;

            ViewData["Quyen"] = new SelectList(_context.Roles, "RoleId", "RoleName", CatID);
            return View(models);
        }

        public IActionResult Filtter(int CatID = 0)
        {
            var url = $"/Admin/AdminAccounts?CatID={CatID}";
            if (CatID == 0)
            {
                url = $"/Admin/AdminAccounts";
            }
            return Json(new { status = "success", redirectUrl = url });
        }

        // GET: Admin/AdminAccounts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var account = await _context.Accounts
                .Include(a => a.Role)
                .FirstOrDefaultAsync(m => m.AccountId == id);
            if (account == null)
            {
                return NotFound();
            }

            ViewData["Quyen"] = new SelectList(_context.Roles, "RoleId", "RoleName");

            return View(account);
        }

        // GET: Admin/AdminAccounts/Create
        public IActionResult Create()
        {
            ViewData["RoleId"] = new SelectList(_context.Roles, "RoleId", "RoleId");
            return View();
        }

        // POST: Admin/AdminAccounts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AccountId,Phone,Email,FullName,Active,Salt,Password,RoleId,CreateDate")] Account account)
        {
            if (ModelState.IsValid)
            {
                _context.Add(account);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["RoleId"] = new SelectList(_context.Roles, "RoleId", "RoleId", account.RoleId);
            ViewData["Quyen"] = new SelectList(_context.Roles, "RoleId", "RoleName");
            return View(account);
        }

        // GET: Admin/AdminAccounts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var account = await _context.Accounts.FindAsync(id);
            if (account == null)
            {
                return NotFound();
            }
            ViewData["RoleId"] = new SelectList(_context.Roles, "RoleId", "RoleId", account.RoleId);
            ViewData["Quyen"] = new SelectList(_context.Roles, "RoleId", "RoleName");
            return View(account);
        }

        // POST: Admin/AdminAccounts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AccountId,Phone,Email,FullName,Active,Salt,Password,RoleId,CreateDate")] Account account)
        {
            if (id != account.AccountId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(account);
                    await _context.SaveChangesAsync();
                    _notifyService.Success("Cập nhật thành công!");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AccountExists(account.AccountId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["RoleId"] = new SelectList(_context.Roles, "RoleId", "RoleId", account.RoleId);
            ViewData["Quyen"] = new SelectList(_context.Roles, "RoleId", "RoleName");
            return View(account);
        }

        // GET: Admin/AdminAccounts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var account = await _context.Accounts
                .Include(a => a.Role)
                .FirstOrDefaultAsync(m => m.AccountId == id);
            if (account == null)
            {
                return NotFound();
            }

            return View(account);
        }

        // POST: Admin/AdminAccounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var account = await _context.Accounts.FindAsync(id);
            _context.Accounts.Remove(account);
            await _context.SaveChangesAsync();
            _notifyService.Error("Xóa tài khoản thành công!");
            return RedirectToAction(nameof(Index));
        }

        private bool AccountExists(int id)
        {
            return _context.Accounts.Any(e => e.AccountId == id);
        }
    }
}
