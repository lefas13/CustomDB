using CustomMVC.Service;
using Microsoft.AspNetCore.Mvc;

namespace CustomMVC.Controllers
{
    public class FeesController : Controller
    {
        private readonly CachedDataService _cachedDataService;

        public FeesController(CachedDataService cachedDataService)
        {
            _cachedDataService = cachedDataService;
        }

        // GET: Agents
        public async Task<IActionResult> Index()
        {
            return View(_cachedDataService.GetFees());
        }
    }
}
