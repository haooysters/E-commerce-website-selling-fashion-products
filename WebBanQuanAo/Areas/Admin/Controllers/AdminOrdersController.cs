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

    [Authorize(Roles = "Admin, Staff")]

    public class AdminOrdersController : Controller
    {
        private readonly db_ClothingContext _context;

        public INotyfService _notyfService { get; }
        public AdminOrdersController(db_ClothingContext context, INotyfService notyfService)
        {
            _context = context;
            _notyfService = notyfService;
        }

        // GET: Admin/AdminOrders
        public IActionResult Index(int page = 1, int CatID = 0)
        {
            var pageNumber = page;
            var pageSize = 5;
            List<Order> lsCourses = new List<Order>();
            if (CatID != 0)
            {
                lsCourses = _context.Orders
                .AsNoTracking()
                .Where(x => x.TransactStatusId == CatID)
                .Include(x => x.Account)
                .Include(x => x.TransactStatus)
                .OrderByDescending(x => x.OrderId).ToList();
            }
            else
            {
                lsCourses = _context.Orders
                .AsNoTracking()
                  .Include(x => x.Account)
                .Include(x => x.TransactStatus)
                .OrderByDescending(x => x.OrderId).ToList();
            }

            PagedList<Order> models = new PagedList<Order>(lsCourses.AsQueryable(), pageNumber, pageSize);

            ViewBag.CurrentCateID = CatID;
            ViewBag.CurrentPage = pageNumber;

            ViewData["TrangThai"] = new SelectList(_context.TransactStatuses, "TransactStatusID", "Status");
            return View(models);
        }

        // GET: Admin/AdminOrders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.Account)
                .Include(o => o.TransactStatus)

                .FirstOrDefaultAsync(m => m.OrderId == id);
            if (order == null)
            {
                return NotFound();
            }

            var Chitietdonhang = _context.OrderDetails
               .Include(x => x.Product)
               .Include(x => x.Size)
               .AsNoTracking()
               .Where(x => x.OrderId == order.OrderId)
               .OrderBy(x => x.OrderDetailId)
               .ToList();
            ViewBag.ChiTiet = Chitietdonhang;

            return View(order);
        }


        public async Task<IActionResult> ChangeStatus(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .AsNoTracking()
                .Include(x => x.Account)
                .FirstOrDefaultAsync(x => x.OrderId == id);
            if (order == null)
            {
                return NotFound();
            }
            ViewData["Trangthai"] = new SelectList(_context.TransactStatuses, "TransactStatusId", "Status", order.TransactStatusId);
            return PartialView("ChangeStatus", order);
        }

        [HttpPost]
        public async Task<IActionResult> ChangeStatus(int id, [Bind("OrderId,AccountId,TransactStatusId,Paid,Deleted,TotalMoney,CreateDate")] Order order)
        {
            if (id != order.OrderId)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    var donhang = await _context.Orders.AsNoTracking().Include(x => x.Account).FirstOrDefaultAsync(x => x.OrderId == id);
                    if (donhang != null)
                    {
                        donhang.Paid = order.Paid;
                        //donhang.Deleted = order.Deleted;
                        donhang.TransactStatusId = order.TransactStatusId;
                        if (donhang.Paid == true)
                        {
                            //donhang.PaymentDate = DateTime.Now;
                        }
                        //if (donhang.TransactStatusId == 5) //donhang.Deleted = true;
                        //if (donhang.TransactStatusId == 3) donhang.ShipDate = DateTime.Now;
                    }
                    _context.Update(donhang);
                    await _context.SaveChangesAsync();
                    _notyfService.Success("Cập nhật trạng thái đơn hàng thành công");

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.OrderId))
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
            ViewData["Trangthai"] = new SelectList(_context.TransactStatuses, "TransactStatusId", "Status", order.TransactStatusId);
            return PartialView("ChangeStatus", order);
        }

        // GET: Admin/AdminOrders/Create
        public IActionResult Create()
        {
            ViewData["AccountId"] = new SelectList(_context.Accounts, "AccountId", "AccountId");
            ViewData["TransactStatusId"] = new SelectList(_context.TransactStatuses, "TransactStatusId", "TransactStatusId");
            return View();
        }

        // POST: Admin/AdminOrders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("OrderId,AccountId,TransactStatusId,Paid,Deleted,TotalMoney,CreateDate")] Order order)
        {
            if (ModelState.IsValid)
            {
                _context.Add(order);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AccountId"] = new SelectList(_context.Accounts, "AccountId", "AccountId", order.AccountId);
            ViewData["TransactStatusId"] = new SelectList(_context.TransactStatuses, "TransactStatusId", "TransactStatusId", order.TransactStatusId);
            return View(order);
        }

        // GET: Admin/AdminOrders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            ViewData["AccountId"] = new SelectList(_context.Accounts, "AccountId", "AccountId", order.AccountId);
            ViewData["TrangThai"] = new SelectList(_context.TransactStatuses, "TransactStatusId", "Status");
            return View(order);
        }

        // POST: Admin/AdminOrders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("OrderId,AccountId,TransactStatusId,Paid,Deleted,TotalMoney,CreateDate,ShipDate")] Order order)
        {
            if (id != order.OrderId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    //var donhang = await _context.Orders.AsNoTracking().Include(x => x.Account).FirstOrDefaultAsync(x => x.OrderId == id);
                    //if (donhang != null)
                    //{
                    //    //donhang.Paid = order.Paid;
                    //    ////donhang.Deleted = order.Deleted;
                    //    //donhang.TransactStatusId = order.TransactStatusId;
                    //    //if (donhang.Paid == true)
                    //    //{
                    //    //    //donhang.PaymentDate = DateTime.Now;
                    //    //}
                    //    ////if (donhang.TransactStatusId == 5) //donhang.Deleted = true;
                    //    if (donhang.TransactStatusId == 3) donhang.ShipDate = DateTime.Now;
                    //}
                    //_context.Update(donhang);           
                    if (order.TransactStatusId == 3) order.ShipDate = DateTime.Now;
                    
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                    _notyfService.Success("Cập nhật trạng thái đơn hàng thành công");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.OrderId))
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
            ViewData["AccountId"] = new SelectList(_context.Accounts, "AccountId", "AccountId", order.AccountId);
            ViewData["TrangThai"] = new SelectList(_context.TransactStatuses, "TransactStatusId", "Status");
            return View(order);
        }

        // GET: Admin/AdminOrders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.Account)
                .Include(o => o.TransactStatus)
                .FirstOrDefaultAsync(m => m.OrderId == id);
            if (order == null)
            {
                return NotFound();
            }

            var Chitietdonhang = _context.OrderDetails
               .Include(x => x.Product)
               .Include(x => x.Size)
               .AsNoTracking()
               .Where(x => x.OrderId == order.OrderId)
               .OrderBy(x => x.OrderDetailId)
               .ToList();
            ViewBag.ChiTiet = Chitietdonhang;

            return View(order);
        }

        // POST: Admin/AdminOrders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.OrderId == id);
        }
    }
}
