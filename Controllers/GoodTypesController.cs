using CustomMVC.Service;
using Microsoft.AspNetCore.Mvc;

namespace CustomMVC.Controllers
{
    public class GoodTypesController : Controller
    {
        private readonly CachedDataService _cachedDataService;

        public GoodTypesController(CachedDataService cachedDataService)
        {
            _cachedDataService = cachedDataService;
        }

        // GET: Agents
        public async Task<IActionResult> Index()
        {
            return View(_cachedDataService.GetGoodTypes());
        }
    }
}
