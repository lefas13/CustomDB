using lab6.Data;
using lab6.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;   // ��� ��������� Swagger

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
        [SwaggerOperation(Summary = "�������� ������ ������", Description = "���������� ��� �������, ������� ���������� � ������, ������ � ������.")]
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
        [SwaggerOperation(Summary = "�������� ������� �� ID", Description = "���������� ���������� � ���������� ������� �� � ID.")]
        [SwaggerResponse(200, "������� �������", typeof(Fee))]
        [SwaggerResponse(404, "������� �� �������")]
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
        [SwaggerOperation(Summary = "�������� �������", Description = "��������� ���������� � ������� �� � ID.")]
        [SwaggerResponse(204, "�������� ���������� �������")]
        [SwaggerResponse(400, "������������ ������")]
        [SwaggerResponse(404, "������� �� ������")]
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
        [SwaggerOperation(Summary = "������� ����� �������", Description = "������� ����� ������� � ���� ������.")]
        [SwaggerResponse(201, "������� ������� �������", typeof(Fee))]
        public async Task<ActionResult<Fee>> PostFee(Fee fee)
        {
            _context.Fees.Add(fee);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetFee", new { id = fee.FeeId }, fee);
        }

        // DELETE: api/Fees/5
        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "������� �������", Description = "������� ������� �� � ID.")]
        [SwaggerResponse(204, "������� ������� �������")]
        [SwaggerResponse(404, "������� �� �������")]
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