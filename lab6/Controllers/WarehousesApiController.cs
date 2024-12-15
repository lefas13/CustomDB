using lab6.Data;
using lab6.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;

namespace lab6.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WarehousesApiController : Controller
    {
        private readonly CustomContext _context;

        public WarehousesApiController(CustomContext context)
        {
            _context = context;
        }

        // GET: api/Warehouses
        [HttpGet]
        [SwaggerOperation(Summary = "Получить список складов", Description = "Возвращает список складов, включая их базовую информацию.")]
        [SwaggerResponse(200, "Список складов успешно получен", typeof(IEnumerable<Warehouse>))]
        public async Task<ActionResult<IEnumerable<Warehouse>>> GetWarehouses()
        {
            return await _context.Warehouses
                                 .Take(20)  // Ограничиваем количество складов
                                 .ToListAsync();
        }

        // GET: api/Warehouses/5
        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Получить склад по ID", Description = "Возвращает информацию о складе по его ID.")]
        [SwaggerResponse(200, "Склад найден", typeof(Warehouse))]
        [SwaggerResponse(404, "Склад не найден")]
        public async Task<ActionResult<Warehouse>> GetWarehouse(int id)
        {
            var warehouse = await _context.Warehouses.FindAsync(id);

            if (warehouse == null)
            {
                return NotFound();
            }

            return warehouse;
        }

        // PUT: api/Warehouses/5
        [HttpPut("{id}")]
        [SwaggerOperation(Summary = "Обновить информацию о складе", Description = "Обновляет информацию о складе по его ID.")]
        [SwaggerResponse(204, "Информация о складе успешно обновлена")]
        [SwaggerResponse(400, "Некорректный запрос")]
        [SwaggerResponse(404, "Склад не найден")]
        public async Task<IActionResult> PutWarehouse(int id, Warehouse warehouse)
        {
            if (id != warehouse.WarehouseId)
            {
                return BadRequest();
            }

            _context.Entry(warehouse).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!WarehouseExists(id))
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

        // POST: api/Warehouses
        [HttpPost]
        [SwaggerOperation(Summary = "Создать новый склад", Description = "Создает новый склад в базе данных.")]
        [SwaggerResponse(201, "Склад успешно создан", typeof(Warehouse))]
        public async Task<ActionResult<Warehouse>> PostWarehouse(Warehouse warehouse)
        {
            _context.Warehouses.Add(warehouse);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetWarehouse", new { id = warehouse.WarehouseId }, warehouse);
        }

        // DELETE: api/Warehouses/5
        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Удалить склад", Description = "Удаляет склад по его ID.")]
        [SwaggerResponse(204, "Склад успешно удален")]
        [SwaggerResponse(404, "Склад не найден")]
        public async Task<IActionResult> DeleteWarehouse(int id)
        {
            var warehouse = await _context.Warehouses.FindAsync(id);
            if (warehouse == null)
            {
                return NotFound();
            }

            _context.Warehouses.Remove(warehouse);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool WarehouseExists(int id)
        {
            return _context.Warehouses.Any(w => w.WarehouseId == id);
        }
    }
}
