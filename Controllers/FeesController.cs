using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CustomMVC.Data;
using CustomMVC.Models;
using CustomMVC.Infrastructure.Filters;
using CustomMVC.Infrastructure;
using CustomMVC.ViewModels;
using CustomMVC.ViewModels.FeeViewModel;
using Microsoft.AspNetCore.Authorization;

namespace CustomMVC.Controllers
{
    public class FeesController : Controller
    {
        private readonly CustomContext _context;
        private readonly int pageSize = 10;   // количество элементов на странице

        public FeesController(CustomContext context, IConfiguration appConfig = null)
        {
            _context = context;
            if (appConfig != null)
            {
                pageSize = int.Parse(appConfig["Parameters:PageSize"]);
            }
        }

        // GET: Fees
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 244)]
        [Authorize]
        [SetToSession("Fees")]
        public async Task<IActionResult> Index(FilterFeeViewModel fee, SortState sortOrder = SortState.No, int page = 1)
        {
            if (fee.FullName == null && fee.Good == null && fee.WarehouseNumber == null && fee.Amount == null && fee.FeeAmount == null &&
                fee.DocumentNumber == null && fee.ReceiptDate == null && fee.PaymentDate == null && fee.ExportDate == null)
            {
                if (HttpContext != null)
                {
                    var sessionFee = Infrastructure.SessionExtensions.Get(HttpContext.Session, "Fees");

                    if (sessionFee != null)
                        fee = Transformations.DictionaryToObject<FilterFeeViewModel>(sessionFee);
                }
            }
            // Сортировка и фильтрация данных
            IQueryable<Fee> feesContext = _context.Fees
            .Include(c => c.Agent) // Загружаем связанные записи Agent
            .Include(c => c.Good) // Загружаем связанные записи Good
            .Include(c => c.Warehouse); // Загружаем связанные записи Warehouse
            feesContext = Sort_Search(feesContext, sortOrder, fee.FullName ?? "", fee.Good ?? "", fee.WarehouseNumber ?? "",
                fee.DocumentNumber ?? "", fee.ReceiptDate ?? null, fee.PaymentDate ?? null, fee.ExportDate ?? null);

            // Разбиение на страницы
            var count = feesContext.Count();
            feesContext = feesContext.Skip((page - 1) * pageSize).Take(pageSize);

            // Формирование модели для передачи представлению
            FeeViewModel fees = new()
            {
                Fees = feesContext,
                PageViewModel = new PageViewModel(count, page, pageSize),
                SortViewModel = new SortViewModel(sortOrder),
                FilterFeeViewModel = fee
            };

            return View(fees);
        }

        // GET: Fees/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fee = await _context.Fees
                .Include(f => f.Agent)
                .Include(f => f.Good)
                .Include(f => f.Warehouse)
                .FirstOrDefaultAsync(m => m.FeeId == id);
            if (fee == null)
            {
                return NotFound();
            }

            return View(fee);
        }

        // GET: Fees/Create
        [Authorize(Roles = "admin")]
        public IActionResult Create()
        {
            ViewData["AgentId"] = new SelectList(_context.Agents, "AgentId", "FullName");
            ViewData["GoodId"] = new SelectList(_context.Goods, "GoodId", "Name");
            ViewData["WarehouseId"] = new SelectList(_context.Warehouses, "WarehouseId", "WarehouseNumber");
            return View();
        }

        // POST: Fees/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Create([Bind("FeeId,WarehouseId,GoodId,ReceiptDate,Amount,DocumentNumber,AgentId,FeeAmount,PaymentDate,ExportDate")] Fee fee)
        {
            if (ModelState.IsValid)
            {
                _context.Add(fee);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(fee);
        }

        // GET: Fees/Edit/5
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fee = await _context.Fees.FindAsync(id);
            if (fee == null)
            {
                return NotFound();
            }
            ViewData["AgentId"] = new SelectList(_context.Agents, "AgentId", "FullName", fee.AgentId);
            ViewData["GoodId"] = new SelectList(_context.Goods, "GoodId", "Name", fee.GoodId);
            ViewData["WarehouseId"] = new SelectList(_context.Warehouses, "WarehouseId", "WarehouseNumber", fee.WarehouseId);
            return View(fee);
        }

        // POST: Fees/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Edit(int id, [Bind("FeeId,WarehouseId,GoodId,ReceiptDate,Amount,DocumentNumber,AgentId,FeeAmount,PaymentDate,ExportDate")] Fee fee)
        {
            if (id != fee.FeeId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(fee);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FeeExists(fee.FeeId))
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
            ViewData["AgentId"] = new SelectList(_context.Agents, "AgentId", "FullName", fee.AgentId);
            ViewData["GoodId"] = new SelectList(_context.Goods, "GoodId", "Name", fee.GoodId);
            ViewData["WarehouseId"] = new SelectList(_context.Warehouses, "WarehouseId", "WarehouseNumber", fee.WarehouseId);
            return View(fee);
        }

        // GET: Fees/Delete/5
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fee = await _context.Fees
                .Include(f => f.Agent)
                .Include(f => f.Good)
                .Include(f => f.Warehouse)
                .FirstOrDefaultAsync(m => m.FeeId == id);
            if (fee == null)
            {
                return NotFound();
            }

            return View(fee);
        }

        // POST: Fees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var fee = await _context.Fees.FindAsync(id);
            if (fee != null)
            {
                _context.Fees.Remove(fee);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FeeExists(int id)
        {
            return _context.Fees.Any(e => e.FeeId == id);
        }

        private static IQueryable<Fee> Sort_Search(IQueryable<Fee> fees, SortState sortOrder, string FullName, string Good, string WarehouseNumber, string DocumentNumber, DateOnly? ReceiptDate, DateOnly? PaymentDate, DateOnly? ExportDate)
        {
            if (!string.IsNullOrEmpty(FullName))
            {
                fees = fees.Where(o => o.Agent.FullName.Contains(FullName ?? ""));
            }

            if (!string.IsNullOrEmpty(Good))
            {
                fees = fees.Where(o => o.Good.Name.Contains(Good ?? ""));
            }

            if (!string.IsNullOrEmpty(WarehouseNumber))
            {
                fees = fees.Where(o => o.Warehouse.WarehouseNumber.Contains(WarehouseNumber ?? ""));
            }

            if (!string.IsNullOrEmpty(DocumentNumber))
            {
                fees = fees.Where(o => o.DocumentNumber.Contains(DocumentNumber ?? ""));
            }

            if (ReceiptDate.HasValue)
            {
                fees = fees.Where(o => o.ReceiptDate == ReceiptDate.Value);
            }

            if (PaymentDate.HasValue)
            {
                fees = fees.Where(o => o.PaymentDate == PaymentDate.Value);
            }

            if (ExportDate.HasValue)
            {
                fees = fees.Where(o => o.ExportDate == ExportDate.Value);
            }

            switch (sortOrder)
            {
                case SortState.FullNameAsc:
                    fees = fees.OrderBy(s => s.Agent.FullName);
                    break;
                case SortState.FullNameDesc:
                    fees = fees.OrderByDescending(s => s.Agent.FullName);
                    break;
                case SortState.GoodAsc:
                    fees = fees.OrderBy(s => s.Good.Name);
                    break;
                case SortState.GoodDesc:
                    fees = fees.OrderByDescending(s => s.Good.Name);
                    break;
                case SortState.WarehouseNumberAsc:
                    fees = fees.OrderBy(s => s.Warehouse.WarehouseNumber);
                    break;
                case SortState.WarehouseNumberDesc:
                    fees = fees.OrderByDescending(s => s.Warehouse.WarehouseNumber);
                    break;
                case SortState.DocumentNumberAsc:
                    fees = fees.OrderBy(s => s.DocumentNumber);
                    break;
                case SortState.DocumentNumberDesc:
                    fees = fees.OrderByDescending(s => s.DocumentNumber);
                    break;
                case SortState.ReceiptDateAsc:
                    fees = fees.OrderBy(s => s.ReceiptDate);
                    break;
                case SortState.ReceiptDateDesc:
                    fees = fees.OrderByDescending(s => s.ReceiptDate);
                    break;
                case SortState.PaymentDateAsc:
                    fees = fees.OrderBy(s => s.PaymentDate);
                    break;
                case SortState.PaymentDateDesc:
                    fees = fees.OrderByDescending(s => s.PaymentDate);
                    break;
                case SortState.ExportDateAsc:
                    fees = fees.OrderBy(s => s.ExportDate);
                    break;
                case SortState.ExportDateDesc:
                    fees = fees.OrderByDescending(s => s.ExportDate);
                    break;
            }
            return fees;
        }
    }
}
