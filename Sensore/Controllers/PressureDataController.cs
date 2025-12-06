using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sensore.Data;
using Sensore.Models;
using Sensore.Services;

namespace Sensore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PressureDataController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly PressureAnalysisService _analysis;

        public PressureDataController(ApplicationDbContext db, PressureAnalysisService analysis)
        {
            _db = db;
            _analysis = analysis;
        }

        // GET: api/pressuredata/latest/{userId}
        [HttpGet("latest/{userId:int}")]
        public async Task<ActionResult<PressureFrame>> GetLatest(int userId)
        {
            var frame = await _db.PressureFrames
                .Where(p => p.UserId == userId)
                .OrderByDescending(p => p.Timestamp)
                .FirstOrDefaultAsync();

            if (frame == null) return NotFound();
            return frame;
        }

        // POST: api/pressuredata/{userId}
        [HttpPost("{userId:int}")]
        public async Task<ActionResult<PressureFrame>> PostFrame(int userId, [FromBody] int[][] matrix)
        {
            if (matrix == null || matrix.Length == 0)
                return BadRequest("Matrix is required.");

            var peak = _analysis.CalculatePeakPressure(matrix);
            var area = _analysis.CalculateContactArea(matrix);
            var risk = _analysis.CalculateRiskScore(matrix);

            var frame = new PressureFrame
            {
                UserId = userId,
                Timestamp = DateTime.UtcNow,
                PressureMatrix = System.Text.Json.JsonSerializer.Serialize(matrix),
                PeakPressure = peak,
                ContactAreaPercent = area,
                RiskScore = risk,
                IsAlerted = risk > 7.5,
                AlertMessage = risk > 7.5 ? "High risk detected" : null
            };

            _db.PressureFrames.Add(frame);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetLatest), new { userId }, frame);
        }
        [HttpPost("analyze-frame")]
        public ActionResult<object> AnalyzeFrame([FromBody] int[][] matrix)
        {
            if (matrix == null || matrix.Length != 32 || matrix.Any(r => r.Length != 32))
                return BadRequest("Matrix must be 32x32.");

            var peak = _analysis.CalculatePeakPressure(matrix);
            var area = _analysis.CalculateContactArea(matrix);
            var risk = _analysis.CalculateRiskScore(matrix);

            return Ok(new
            {
                PeakPressure = peak,
                ContactArea = area,
                RiskScore = risk,
                Matrix = matrix
            });
        } 
    }
}
