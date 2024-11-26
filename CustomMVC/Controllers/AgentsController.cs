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
using CustomMVC.ViewModels.AgentViewModel;
using CustomMVC.Infrastructure.Filters;
using CustomMVC.Infrastructure;
using Microsoft.AspNetCore.Authorization;

namespace CustomMVC.Controllers
{
    public class AgentsController : Controller
    {
        private readonly CustomContext _context;
        private readonly int pageSize = 10;   // количество элементов на странице

        public AgentsController(CustomContext context, IConfiguration appConfig = null)
        {
            _context = context;
            if (appConfig != null)
            {
                pageSize = int.Parse(appConfig["Parameters:PageSize"]);
            }
        }

        // GET: Agents
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 244)]
        [Authorize]
        [SetToSession("Agents")]
        public IActionResult Index(FilterAgentViewModel agent, SortState sortOrder = SortState.No, int page = 1)
        {
            if (agent.FullName == null && agent.IdNumber == null)
            {
                if (HttpContext != null)
                {
                    var sessionAgent = Infrastructure.SessionExtensions.Get(HttpContext.Session, "Agents");

                    if (sessionAgent != null)
                        agent = Transformations.DictionaryToObject<FilterAgentViewModel>(sessionAgent);
                }
            }

            // Сортировка и фильтрация данных
            IQueryable<Agent> agentsContext = _context.Agents;
            agentsContext = Sort_Search(agentsContext, sortOrder, agent.FullName ?? "", agent.IdNumber ?? "");

            // Разбиение на страницы
            var count = agentsContext.Count();
            agentsContext = agentsContext.Skip((page - 1) * pageSize).Take(pageSize);

            // Формирование модели для передачи представлению
            AgentViewModel agents = new()
            {
                Agents = agentsContext,
                PageViewModel = new PageViewModel(count, page, pageSize),
                SortViewModel = new SortViewModel(sortOrder),
                FilterAgentViewModel = agent
            };

            return View(agents);
        }
        // GET: Agents/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var agent = await _context.Agents
                .FirstOrDefaultAsync(m => m.AgentId == id);
            if (agent == null)
            {
                return NotFound();
            }

            return View(agent);
        }

        // GET: Agents/Create
        [Authorize(Roles = "admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Agents/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Create([Bind("AgentId,FullName,IdNumber")] Agent agent)
        {
            if (ModelState.IsValid)
            {
                _context.Add(agent);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(agent);
        }

        // GET: Agents/Edit/5
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var agent = await _context.Agents.FindAsync(id);
            if (agent == null)
            {
                return NotFound();
            }
            return View(agent);
        }

        // POST: Agents/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Edit(int id, [Bind("AgentId,FullName,IdNumber")] Agent agent)
        {
            if (id != agent.AgentId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(agent);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AgentExists(agent.AgentId))
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
            return View(agent);
        }

        // GET: Agents/Delete/5
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var agent = await _context.Agents
                .FirstOrDefaultAsync(m => m.AgentId == id);
            if (agent == null)
            {
                return NotFound();
            }

            return View(agent);
        }

        // POST: Agents/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var agent = await _context.Agents.FindAsync(id);
            if (agent != null)
            {
                _context.Agents.Remove(agent);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AgentExists(int id)
        {
            return _context.Agents.Any(e => e.AgentId == id);
        }

        private static IQueryable<Agent> Sort_Search(IQueryable<Agent> agents, SortState sortOrder, string FullName, string IdNumber)
        {
            if (!string.IsNullOrEmpty(FullName))
            {
                agents = agents.Where(o => o.FullName.Contains(FullName ?? ""));
            }

            if (!string.IsNullOrEmpty(IdNumber))
            {
                agents = agents.Where(o => o.IdNumber.Contains(IdNumber ?? ""));
            }

            switch (sortOrder)
            {
                case SortState.FullNameAsc:
                    agents = agents.OrderBy(s => s.FullName);
                    break;
                case SortState.FullNameDesc:
                    agents = agents.OrderByDescending(s => s.FullName);
                    break;
                case SortState.IdNumberAsc:
                    agents = agents.OrderBy(s => s.IdNumber);
                    break;
                case SortState.IdNumberDesc:
                    agents = agents.OrderByDescending(s => s.IdNumber);
                    break;
            }
            agents = agents.Where(o => o.FullName.Contains(FullName ?? ""));
            return agents;
        }
    }
}
