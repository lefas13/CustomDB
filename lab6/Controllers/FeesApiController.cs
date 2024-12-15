using lab6.Data;
using lab6.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;   // Для аннотаций Swagger

namespace lab6.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FeesApiController : Controller
    {
        private readonly CustomContext _context;

        public FeesApiController(CustomContext context)
        {
            _context = context;
        }



        // GET: api/Fees
        [HttpGet]
        [SwaggerOperation(Summary = "Получить список пошлин", Description = "Возвращает все пошлины, включая информацию о товаре, складе и агенте.")]
        public async Task<ActionResult<IEnumerable<Fee>>> GetFees()
        {
            return await _context.Fees
                .Include(f => f.Agent)
                .Include(f => f.Warehouse)
                .Include(f => f.Good)
                .Take(20)
                .ToListAsync();
        }

        // GET: api/Fees/5
        // BY Id
        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Получить пошлину по ID", Description = "Возвращает информацию о конкретной пошлине по её ID.")]
        [SwaggerResponse(200, "Пошлина найдена", typeof(Fee))]
        [SwaggerResponse(404, "Пошлина не найдена")]
        public async Task<ActionResult<Fee>> GetFee(int id)
        {
            var fee = await _context.Fees
                .Include(f => f.Agent)
                .Include(f => f.Warehouse)
                .Include(f => f.Good)
                .FirstOrDefaultAsync(f => f.FeeId == id);

            if (fee == null)
            {
                return NotFound();
            }

            return fee;
        }

        // PUT: api/Fees/5
        [HttpPut("{id}")]
        [SwaggerOperation(Summary = "Обновить пошлину", Description = "Обновляет информацию о пошлине по её ID.")]
        [SwaggerResponse(204, "Успешное обновление пошлины")]
        [SwaggerResponse(400, "Некорректный запрос")]
        [SwaggerResponse(404, "Пошлина не найден")]
        public async Task<IActionResult> PutFee(int id, Fee fee)
        {
            if (id != fee.FeeId)
            {
                return BadRequest();
            }

            _context.Entry(fee).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FeeExists(id))
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

        // POST: api/Fees
        [HttpPost]
        [SwaggerOperation(Summary = "Создать новую пошлину", Description = "Создает новую пошлину в базе данных.")]
        [SwaggerResponse(201, "Пошлина успешно создана", typeof(Fee))]
        public async Task<ActionResult<Fee>> PostFee(Fee fee)
        {
            _context.Fees.Add(fee);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetFee", new { id = fee.FeeId }, fee);
        }

        // DELETE: api/Fees/5
        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Удалить пошлину", Description = "Удаляет пошлину по её ID.")]
        [SwaggerResponse(204, "Пошлина успешно удалена")]
        [SwaggerResponse(404, "Пошлина не найдена")]
        public async Task<IActionResult> DeleteFee(int id)
        {
            var fee = await _context.Fees
                .Include(f => f.Agent)
                .Include(f => f.Warehouse)
                .Include(f => f.Good)
                .FirstOrDefaultAsync(f => f.FeeId == id);

            if (fee == null)
            {
                return NotFound();
            }

            _context.Fees.Remove(fee);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool FeeExists(int id)
        {
            return _context.Fees.Any(f => f.FeeId == id);
        }
    }
}