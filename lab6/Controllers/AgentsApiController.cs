using lab6.Data;
using lab6.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;

namespace lab6.Controllers
{
        [Route("api/[controller]")]
        [ApiController]
        public class AgentsApiController : Controller
        {
            private readonly CustomContext _context;

            public AgentsApiController(CustomContext context)
            {
                _context = context;
            }

            // GET: api/Agents
            [HttpGet]
            [SwaggerOperation(Summary = "Получить список агентов", Description = "Возвращает список агентов, включая их базовую информацию.")]
            [SwaggerResponse(200, "Список агентов успешно получен", typeof(IEnumerable<Agent>))]
            public async Task<ActionResult<IEnumerable<Agent>>> GetAgents()
            {
                return await _context.Agents
                                     .Take(20)  // Ограничиваем количество агентов
                                     .ToListAsync();
            }

            // GET: api/Agents/5
            [HttpGet("{id}")]
            [SwaggerOperation(Summary = "Получить агента по ID", Description = "Возвращает информацию об агенте по его ID.")]
            [SwaggerResponse(200, "Агент найден", typeof(Agent))]
            [SwaggerResponse(404, "Агент не найден")]
            public async Task<ActionResult<Agent>> GetAgent(int id)
            {
                var agent = await _context.Agents.FindAsync(id);

                if (agent == null)
                {
                    return NotFound();
                }

                return agent;
            }

            // PUT: api/Agents/5
            [HttpPut("{id}")]
            [SwaggerOperation(Summary = "Обновить информацию об агенте", Description = "Обновляет информацию об агенте по его ID.")]
            [SwaggerResponse(204, "Информация об агенте успешно обновлена")]
            [SwaggerResponse(400, "Некорректный запрос")]
            [SwaggerResponse(404, "Агент не найден")]
            public async Task<IActionResult> PutAgent(int id, Agent agent)
            {
                if (id != agent.AgentId)
                {
                    return BadRequest();
                }

                _context.Entry(agent).State = EntityState.Modified;

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AgentExists(id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return NoContent();
            }

            // POST: api/Agents
            [HttpPost]
            [SwaggerOperation(Summary = "Создать нового агента", Description = "Создает нового агента в базе данных.")]
            [SwaggerResponse(201, "Агент успешно создан", typeof(Agent))]
            public async Task<ActionResult<Agent>> PostAgent(Agent agent)
            {
                _context.Agents.Add(agent);
                await _context.SaveChangesAsync();

                return CreatedAtAction("GetAgent", new { id = agent.AgentId }, agent);
            }

            // DELETE: api/Agents/5
            [HttpDelete("{id}")]
            [SwaggerOperation(Summary = "Удалить агента", Description = "Удаляет агента по его ID.")]
            [SwaggerResponse(204, "Агент успешно удален")]
            [SwaggerResponse(404, "Агент не найден")]
            public async Task<IActionResult> DeleteAgent(int id)
            {
                var agent = await _context.Agents.FindAsync(id);
                if (agent == null)
                {
                    return NotFound();
                }

                _context.Agents.Remove(agent);
                await _context.SaveChangesAsync();

                return NoContent();
            }

            private bool AgentExists(int id)
            {
                return _context.Agents.Any(a => a.AgentId == id);
            }
        }
    }
