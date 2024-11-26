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
using System.Drawing.Printing;
using CustomMVC.ViewModels.GoodViewModel;
using Microsoft.Identity.Client;
using CustomMVC.ViewModels.AgentViewModel;
using Microsoft.AspNetCore.Authorization;

namespace CustomMVC.Controllers
{
    public class GoodsController : Controller
    {
        private readonly CustomContext _context;
        private readonly int pageSize = 10;   // количество элементов на странице

        public GoodsController(CustomContext context, IConfiguration appConfig = null)
        {
            _context = context;
            if (appConfig != null)
            {
                pageSize = int.Parse(appConfig["Parameters:PageSize"]);
            }
        }

        // GET: Goods
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 244)]
        [Authorize]
        [SetToSession("Goods")]
        public async Task<IActionResult> Index(FilterGoodViewModel good, SortState sortOrder = SortState.No, int page = 1)
        {
            if (good.Good == null && good.Type == null)
            {
                if (HttpContext != null)
                {
                    var sessionGood = Infrastructure.SessionExtensions.Get(HttpContext.Session, "Goods");

                    if (sessionGood != null)
                        good = Transformations.DictionaryToObject<FilterGoodViewModel>(sessionGood);
                }
            }

            // Сортировка и фильтрация данных
            IQueryable<Good> goodsContext = _context.Goods;
            goodsContext = Sort_Search(goodsContext, sortOrder, good.Good ?? "", good.Type ?? "");

            // Разбиение на страницы
            var count = goodsContext.Count();
            goodsContext = goodsContext.Skip((page - 1) * pageSize).Take(pageSize);

            // Формирование модели для передачи представлению
            GoodViewModel goods = new()
            {
                Goods = goodsContext,
                PageViewModel = new PageViewModel(count, page, pageSize),
                SortViewModel = new SortViewModel(sortOrder),
                FilterGoodViewModel = good
            };

            return View(goods);
        }

        // GET: Goods/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var good = await _context.Goods
                .Include(g => g.GoodType)
                .FirstOrDefaultAsync(m => m.GoodId == id);
            if (good == null)
            {
                return NotFound();
            }

            return View(good);
        }

        // GET: Goods/Create
        [Authorize(Roles = "admin")]
        public IActionResult Create()
        {
            ViewData["GoodTypeId"] = new SelectList(_context.GoodTypes, "GoodTypeId", "Name");
            return View();
        }

        // POST: Goods/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Create([Bind("GoodId,Name,GoodTypeId")] Good good)
        {
            if (ModelState.IsValid)
            {
                _context.Add(good);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(good);
        }

        // GET: Goods/Edit/5
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var good = await _context.Goods.FindAsync(id);
            if (good == null)
            {
                return NotFound();
            }
            ViewData["GoodTypeId"] = new SelectList(_context.GoodTypes, "GoodTypeId", "Name", good.GoodTypeId);
            return View(good);
        }

        // POST: Goods/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Edit(int id, Good good)
        {
            if (id != good.GoodId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(good);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GoodExists(good.GoodId))
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
            ViewData["GoodTypeId"] = new SelectList(_context.GoodTypes, "GoodTypeId", "Name", good.GoodTypeId);
            return View(good);
        }

        // GET: Goods/Delete/5
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var good = await _context.Goods
                .Include(g => g.GoodType)
                .FirstOrDefaultAsync(m => m.GoodId == id);
            if (good == null)
            {
                return NotFound();
            }

            return View(good);
        }

        // POST: Goods/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var good = await _context.Goods.FindAsync(id);
            if (good != null)
            {
                _context.Goods.Remove(good);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GoodExists(int id)
        {
            return _context.Goods.Any(e => e.GoodId == id);
        }

        private static IQueryable<Good> Sort_Search(IQueryable<Good> goods, SortState sortOrder, string Good, string Type)
        {
            if (!string.IsNullOrEmpty(Good))
            {
                goods = goods.Where(o => o.Name.Contains(Good ?? ""));
            }

            if (!string.IsNullOrEmpty(Type))
            {
                goods = goods.Where(o => o.GoodType.Name.Contains(Type ?? ""));
            }

            switch (sortOrder)
            {
                case SortState.GoodAsc:
                    goods = goods.OrderBy(s => s.Name);
                    break;
                case SortState.GoodDesc:
                    goods = goods.OrderByDescending(s => s.Name);
                    break;
                case SortState.TypeAsc:
                    goods = goods.OrderBy(s => s.GoodType.Name);
                    break;
                case SortState.IdNumberDesc:
                    goods = goods.OrderByDescending(s => s.GoodType.Name);
                    break;
            }
            return goods;
        }
    }
}
