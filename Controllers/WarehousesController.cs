using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CustomMVC.Data;
using CustomMVC.Models;
using Microsoft.Identity.Client;
using CustomMVC.Infrastructure.Filters;
using CustomMVC.ViewModels.WarehouseViewModel;
using CustomMVC.ViewModels;
using CustomMVC.Infrastructure;
using CustomMVC.ViewModels.AgentViewModel;
using Microsoft.AspNetCore.Authorization;

namespace CustomMVC.Controllers
{
    public class WarehousesController : Controller
    {
        private readonly CustomContext _context;
        private readonly int pageSize = 10;   // количество элементов на странице

        public WarehousesController(CustomContext context, IConfiguration appConfig = null)
        {
            _context = context;
            {
                pageSize = int.Parse(appConfig["Parameters:PageSize"]);
            }
        }

        // GET: Warehouses
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 244)]
        [Authorize]
        [SetToSession("Warehouses")]
        public async Task<IActionResult> Index(FilterWarehouseViewModel warehouse, SortState sortOrder = SortState.No, int page = 1)
        {
            if (warehouse.WarehouseNumber == null)
            {
                if (HttpContext != null)
                {
                    var sessionWarehouse = Infrastructure.SessionExtensions.Get(HttpContext.Session, "Warehouses");

                    if (sessionWarehouse != null)
                        warehouse = Transformations.DictionaryToObject<FilterWarehouseViewModel>(sessionWarehouse);
                }
            }

            // Сортировка и фильтрация данных
            IQueryable<Warehouse> warehousesContext = _context.Warehouses;
            warehousesContext = Sort_Search(warehousesContext, sortOrder, warehouse.WarehouseNumber ?? "");

            // Разбиение на страницы
            var count = warehousesContext.Count();
            warehousesContext = warehousesContext.Skip((page - 1) * pageSize).Take(pageSize);

            // Формирование модели для передачи представлению
            WarehouseViewModel warehouses = new()
            {
                Warehouses = warehousesContext,
                PageViewModel = new PageViewModel(count, page, pageSize),
                SortViewModel = new SortViewModel(sortOrder),
                FilterWarehouseViewModel = warehouse
            };

            return View(warehouses);
        }

        // GET: Warehouses/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var warehouse = await _context.Warehouses
                .FirstOrDefaultAsync(m => m.WarehouseId == id);
            if (warehouse == null)
            {
                return NotFound();
            }

            return View(warehouse);
        }

        // GET: Warehouses/Create
        [Authorize(Roles = "admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Warehouses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("WarehouseId,WarehouseNumber")] Warehouse warehouse)
        {
            if (ModelState.IsValid)
            {
                _context.Add(warehouse);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(warehouse);
        }

        // GET: Warehouses/Edit/5
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var warehouse = await _context.Warehouses.FindAsync(id);
            if (warehouse == null)
            {
                return NotFound();
            }
            return View(warehouse);
        }

        // POST: Warehouses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Edit(int id, [Bind("WarehouseId,WarehouseNumber")] Warehouse warehouse)
        {
            if (id != warehouse.WarehouseId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(warehouse);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WarehouseExists(warehouse.WarehouseId))
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
            return View(warehouse);
        }

        // GET: Warehouses/Delete/5
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var warehouse = await _context.Warehouses
                .FirstOrDefaultAsync(m => m.WarehouseId == id);
            if (warehouse == null)
            {
                return NotFound();
            }

            return View(warehouse);
        }

        // POST: Warehouses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var warehouse = await _context.Warehouses.FindAsync(id);
            if (warehouse != null)
            {
                _context.Warehouses.Remove(warehouse);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool WarehouseExists(int id)
        {
            return _context.Warehouses.Any(e => e.WarehouseId == id);
        }

        private static IQueryable<Warehouse> Sort_Search(IQueryable<Warehouse> warehouses, SortState sortOrder, string WarehouseNumber)
        {
            if (!string.IsNullOrEmpty(WarehouseNumber))
            {
                warehouses = warehouses.Where(o => o.WarehouseNumber.Contains(WarehouseNumber ?? ""));
            }

            switch (sortOrder)
            {
                case SortState.WarehouseNumberAsc:
                    warehouses = warehouses.OrderBy(s => s.WarehouseNumber);
                    break;
                case SortState.WarehouseNumberDesc:
                    warehouses = warehouses.OrderByDescending(s => s.WarehouseNumber);
                    break;
            }
            return warehouses;
        }
    }
}
