using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CustomMVC.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CustomMVC.Controllers
{
    public class AgentsController : Controller
    {
        private readonly CachedDataService _cachedDataService;

        public AgentsController(CachedDataService cachedDataService)
        {
            _cachedDataService = cachedDataService;
        }

        // GET: Agents
        public async Task<IActionResult> Index()
        {
            return View(_cachedDataService.GetAgents());
        }
    }
}
