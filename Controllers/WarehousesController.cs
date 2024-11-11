using CustomMVC.Service;
using Microsoft.AspNetCore.Mvc;

namespace CustomMVC.Controllers
{
    public class WarehousesController : Controller
    {
        private readonly CachedDataService _cachedDataService;

        public WarehousesController(CachedDataService cachedDataService)
        {
            _cachedDataService = cachedDataService;
        }

        // GET: Agents
        public async Task<IActionResult> Index()
        {
            return View(_cachedDataService.GetWarehouses());
        }
    }
}
