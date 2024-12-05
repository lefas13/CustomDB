using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CustomMVC.Data;
using CustomMVC.Models;
using CustomMVC.ViewModels;
using CustomMVC.ViewModels.CustomInformationViewModel;
using CustomMVC.Infrastructure.Filters;
using CustomMVC.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using CustomMVC.ViewModels.AgentViewModel;

namespace CustomMVC.Controllers
{
    public class CustomInformationController : Controller
    {
        private readonly CustomContext _context;
        private const int PageSize = 10; // Количество товаров на странице

        public CustomInformationController(CustomContext context)
        {
            _context = context;
        }

        // GET: Index
        [Authorize]
        public async Task<IActionResult> Index()
        {
            var viewModel = new CustomInformationViewModel
            {
                Warehouses = await _context.Warehouses.ToListAsync(),
                Agents = await _context.Agents.ToListAsync(),
                SelectedAgentId = 0,
                SelectedWarehouseId = 0
            };

            return View(viewModel);
        }

        // GET: Details
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 244)]
        public async Task<IActionResult> Details(DateOnly startDate, DateOnly endDate, int? selectedWarehouseId, int? selectedAgentId, int pageNumber = 1)
        {
            var viewModel = new CustomInformationViewModel
            {
                Warehouses = await _context.Warehouses.ToListAsync(),
                Agents = await _context.Agents.ToListAsync(),
                SelectedAgentId = selectedAgentId ?? 0,
                SelectedWarehouseId = selectedWarehouseId ?? 0
            };

            // Получаем товары и их данные на выбранном складе
            viewModel.GoodsDetails = new Dictionary<int, List<GoodDetails>>();
            var goodsDetails = await _context.Goods
                .Select(g => new GoodDetails
                {
                    GoodId = g.GoodId,
                    Name = g.Name,
                    Count = _context.Fees
                        .Where(f => f.WarehouseId == viewModel.SelectedWarehouseId && f.GoodId == g.GoodId)
                        .Sum(f => f.Amount)
                }).Where(g => _context.Fees.Any(f => f.WarehouseId == viewModel.SelectedWarehouseId && f.GoodId == g.GoodId)).ToListAsync();

            viewModel.GoodsDetails[viewModel.SelectedWarehouseId] = goodsDetails;

            var goodsQuery = _context.Goods
                .Select(g => new GoodDetails
                {
                    GoodId = g.GoodId,
                    Name = g.Name,
                    Count = _context.Fees
                        .Where(f => f.WarehouseId == viewModel.SelectedWarehouseId && f.GoodId == g.GoodId)
                        .Sum(f => f.Amount)
                });

            // Пагинация
            var totalItems = await goodsQuery.CountAsync() / 10;
            var goods = await _context.Fees
                .OrderBy(f => f.Good.Name) // Сортировка по имени товара
                .Skip((pageNumber - 1) * PageSize)
                .Take(PageSize)
                .GroupBy(f => f.Good.Name)
                .ToDictionaryAsync(g => g.Key, g => g.Sum(f => f.Amount));

            viewModel.TotalGoodsCount = goods;
            viewModel.PagingInfo = new PagingInfo
            {
                CurrentPage = pageNumber,
                ItemsPerPage = PageSize,
                TotalItems = totalItems
            };


            // Количество определенного товара на выбранном складе за заданный период
            viewModel.GoodsCountOnWarehouse = await _context.Fees
                .Where(f => f.WarehouseId == viewModel.SelectedWarehouseId && f.ReceiptDate >= startDate && f.ReceiptDate <= endDate)
                .GroupBy(f => f.Good.Name)
                .ToDictionaryAsync(g => g.Key, g => g.Sum(f => f.Amount));

            // Среднее время нахождения каждого вида товара на выбранном складе
            viewModel.AverageStayTime = await _context.Fees
                .Where(f => f.WarehouseId == viewModel.SelectedWarehouseId)
                .GroupBy(f => f.Good.Name)
                .ToDictionaryAsync(g => g.Key, g =>
                {
                    // Преобразование DateOnly в DateTime для вычислений
                    var averageStayTime = g.Average(f =>
                    {
                    // Преобразуем ReceiptDate в DateTime
                        var receiptDateTime = f.ReceiptDate.ToDateTime(new TimeOnly(0, 0)); // Указание времени на полночь
                        return (DateTime.Now - receiptDateTime).TotalDays;
                    });
                    return averageStayTime;
                });


            // Объем пошлин
            var fees = await _context.Fees
                .Where(f => f.ReceiptDate >= startDate && f.ReceiptDate <= endDate && (selectedAgentId == null || f.AgentId == selectedAgentId))
                .ToListAsync();

            viewModel.TotalFeesForPeriod = fees.Sum(f => f.FeeAmount);

            // Сумма пошлин по выбранному складу и агенту за текущий месяц
            viewModel.FeesByWarehouseAndAgent = await _context.Fees
                .Where(f => f.WarehouseId == viewModel.SelectedWarehouseId && f.AgentId == selectedAgentId && f.PaymentDate.Value.Month == DateTime.Now.Month && f.PaymentDate.Value.Year == DateTime.Now.Year)
                .SumAsync(f => f.FeeAmount);

            // Итоговая сумма всех пошлин за текущий год
            viewModel.TotalFeesForYear = await _context.Fees
                .Where(f => f.PaymentDate.Value.Year == DateTime.Now.Year)
                .SumAsync(f => f.FeeAmount);

            return View("Details", viewModel);
        }
    }
}

