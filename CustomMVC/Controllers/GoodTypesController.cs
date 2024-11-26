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
using CustomMVC.ViewModels.GoodTypeViewModel;
using CustomMVC.ViewModels;
using Microsoft.Data.SqlClient;
using System.Drawing.Printing;
using Microsoft.Identity.Client;
using Microsoft.AspNetCore.Authorization;

namespace CustomMVC.Controllers
{
    public class GoodTypesController : Controller
    {
        private readonly CustomContext _context;
        private readonly int pageSize = 10;   // количество элементов на странице

        public GoodTypesController(CustomContext context, IConfiguration appConfig = null)
        {
            _context = context;
            if (appConfig != null)
            {
                pageSize = int.Parse(appConfig["Parameters:PageSize"]);
            }
        }

        // GET: GoodTypes
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 244)]
        [Authorize]
        [SetToSession("GoodTypes")]
        public async Task<IActionResult> Index(FilterGoodTypeViewModel goodType, SortState sortOrder = SortState.No, int page = 1)
        {
            if (goodType.Type == null && goodType.Measurement == null)
            {
                if (HttpContext != null)
                {
                    var sessionGoodType = Infrastructure.SessionExtensions.Get(HttpContext.Session, "GoodTypes");

                    if (sessionGoodType != null)
                        goodType = Transformations.DictionaryToObject<FilterGoodTypeViewModel>(sessionGoodType);
                }
            }

            // Сортировка и фильтрация данных
            IQueryable<GoodType> goodTypesContext = _context.GoodTypes;
            goodTypesContext = Sort_Search(goodTypesContext, sortOrder, goodType.Type ?? "", goodType.Measurement ?? "");

            // Разбиение на страницы
            var count = goodTypesContext.Count();
            goodTypesContext = goodTypesContext.Skip((page - 1) * pageSize).Take(pageSize);

            // Формирование модели для передачи представлению
            GoodTypeViewModel goodTypes = new()
            {
                GoodTypes = goodTypesContext,
                PageViewModel = new PageViewModel(count, page, pageSize),
                SortViewModel = new SortViewModel(sortOrder),
                FilterGoodTypeViewModel = goodType
            };

            return View(goodTypes);
        }

        // GET: GoodTypes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var goodType = await _context.GoodTypes
                .FirstOrDefaultAsync(m => m.GoodTypeId == id);
            if (goodType == null)
            {
                return NotFound();
            }

            return View(goodType);
        }

        // GET: GoodTypes/Create
        [Authorize(Roles = "admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: GoodTypes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Create([Bind("GoodTypeId,Name,Measurement,AmountOfFee")] GoodType goodType)
        {
            if (ModelState.IsValid)
            {
                _context.Add(goodType);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(goodType);
        }

        // GET: GoodTypes/Edit/5
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var goodType = await _context.GoodTypes.FindAsync(id);
            if (goodType == null)
            {
                return NotFound();
            }
            return View(goodType);
        }

        // POST: GoodTypes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Edit(int id, [Bind("GoodTypeId,Name,Measurement,AmountOfFee")] GoodType goodType)
        {
            if (id != goodType.GoodTypeId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(goodType);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GoodTypeExists(goodType.GoodTypeId))
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
            return View(goodType);
        }

        // GET: GoodTypes/Delete/5
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var goodType = await _context.GoodTypes
                .FirstOrDefaultAsync(m => m.GoodTypeId == id);
            if (goodType == null)
            {
                return NotFound();
            }

            return View(goodType);
        }

        // POST: GoodTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var goodType = await _context.GoodTypes.FindAsync(id);
            if (goodType != null)
            {
                _context.GoodTypes.Remove(goodType);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GoodTypeExists(int id)
        {
            return _context.GoodTypes.Any(e => e.GoodTypeId == id);
        }

        private static IQueryable<GoodType> Sort_Search(IQueryable<GoodType> goodTypes, SortState sortOrder, string Type, string Measurement)
        {
            if (!string.IsNullOrEmpty(Type))
            {
                goodTypes = goodTypes.Where(o => o.Name.Contains(Type ?? ""));
            }

            if (!string.IsNullOrEmpty(Measurement))
            {
                goodTypes = goodTypes.Where(o => o.Measurement.Contains(Measurement ?? ""));
            }

            switch (sortOrder)
            {
                case SortState.TypeAsc:
                    goodTypes = goodTypes.OrderBy(s => s.Name);
                    break;
                case SortState.TypeDesc:
                    goodTypes = goodTypes.OrderByDescending(s => s.Name);
                    break;
                case SortState.MeasurementAsc:
                    goodTypes = goodTypes.OrderBy(s => s.Measurement);
                    break;
                case SortState.IdNumberDesc:
                    goodTypes = goodTypes.OrderByDescending(s => s.Measurement);
                    break;
            }
            return goodTypes;
        }
    }
}
