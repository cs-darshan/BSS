using Microsoft.AspNetCore.Mvc;
using Sensore.Services;
using System.Security.Claims;

namespace Sensore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ImportController : ControllerBase
    {
        public record CsvSession(string UserId, string DisplayName, string Path);

        private static readonly Dictionary<string, CsvSession> CsvSessions = new()
        {
            // User 1 – 7 sessions
            ["u1-s1"] = new CsvSession("user1", "User 1 – Session 1", "/Users/darshannarkhede/RiderProjects/Sensore/Sensore/GTLB-Data/1c0fd777_20251011.csv"),
            ["u1-s2"] = new CsvSession("user1", "User 1 – Session 2", "/Users/darshannarkhede/RiderProjects/Sensore/Sensore/GTLB-Data/1c0fd777_20251012.csv"),
            ["u1-s3"] = new CsvSession("user1", "User 1 – Session 3", "/Users/darshannarkhede/RiderProjects/Sensore/Sensore/GTLB-Data/1c0fd777_20251013.csv"),
            ["u1-s4"] = new CsvSession("user1", "User 1 – Session 4", "/Users/darshannarkhede/RiderProjects/Sensore/Sensore/GTLB-Data/71e66ab3_20251011.csv"),
            ["u1-s5"] = new CsvSession("user1", "User 1 – Session 5", "/Users/darshannarkhede/RiderProjects/Sensore/Sensore/GTLB-Data/71e66ab3_20251012.csv"),
            ["u1-s6"] = new CsvSession("user1", "User 1 – Session 6", "/Users/darshannarkhede/RiderProjects/Sensore/Sensore/GTLB-Data/71e66ab3_20251013.csv"),
            ["u1-s7"] = new CsvSession("user1", "User 1 – Session 7", "/Users/darshannarkhede/RiderProjects/Sensore/Sensore/GTLB-Data/543d4676_20251011.csv"),

            // User 2 – 7 sessions
            ["u2-s1"] = new CsvSession("user2", "User 2 – Session 1", "/Users/darshannarkhede/RiderProjects/Sensore/Sensore/GTLB-Data/543d4676_20251012.csv"),
            ["u2-s2"] = new CsvSession("user2", "User 2 – Session 2", "/Users/darshannarkhede/RiderProjects/Sensore/Sensore/GTLB-Data/543d4676_20251013.csv"),
            ["u2-s3"] = new CsvSession("user2", "User 2 – Session 3", "/Users/darshannarkhede/RiderProjects/Sensore/Sensore/GTLB-Data/d13043b3_20251011.csv"),
            ["u2-s4"] = new CsvSession("user2", "User 2 – Session 4", "/Users/darshannarkhede/RiderProjects/Sensore/Sensore/GTLB-Data/d13043b3_20251012.csv"),
            ["u2-s5"] = new CsvSession("user2", "User 2 – Session 5", "/Users/darshannarkhede/RiderProjects/Sensore/Sensore/GTLB-Data/d13043b3_20251013.csv"),
            ["u2-s6"] = new CsvSession("user2", "User 2 – Session 6", "/Users/darshannarkhede/RiderProjects/Sensore/Sensore/GTLB-Data/de0e9b2c_20251011.csv"),
            ["u2-s7"] = new CsvSession("user2", "User 2 – Session 7", "/Users/darshannarkhede/RiderProjects/Sensore/Sensore/GTLB-Data/de0e9b2c_20251012.csv"),
        };

        public class ImportRequest
        {
            public string SessionId { get; set; } = string.Empty;
        }

        private readonly CsvImportService _csv;

        public ImportController(CsvImportService csv)
        {
            _csv = csv;
        }

        private CsvSession? ResolveSession(string sessionId)
        {
            if (string.IsNullOrWhiteSpace(sessionId)) return null;
            return CsvSessions.TryGetValue(sessionId, out var s) ? s : null;
        }

        private string? GetCurrentUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        // ---------- ADMIN ENDPOINTS (use SessionId explicitly) ----------

        [HttpPost("test")]
        public ActionResult<List<object>> ImportTest([FromBody] ImportRequest request)
        {
            var session = ResolveSession(request.SessionId);
            if (session == null || !System.IO.File.Exists(session.Path))
                return BadRequest("Session not found.");

            var frames = new List<object>();
            int index = 0;

            foreach (var matrix in _csv.ReadFrames(session.Path).Take(5))
            {
                var (peak, area, risk) = _csv.AnalyzeFrame(matrix);
                frames.Add(new
                {
                    FrameIndex = index++,
                    PeakPressure = peak,
                    ContactArea = area,
                    RiskScore = risk
                });
            }

            return Ok(frames);
        }

        [HttpPost("timeline")]
        public ActionResult<List<object>> ImportTimeline([FromBody] ImportRequest request)
        {
            var session = ResolveSession(request.SessionId);
            if (session == null || !System.IO.File.Exists(session.Path))
                return BadRequest("Session not found.");

            var points = new List<object>();
            int index = 0;

            foreach (var matrix in _csv.ReadFrames(session.Path))
            {
                var (peak, area, risk) = _csv.AnalyzeFrame(matrix);
                points.Add(new
                {
                    FrameIndex = index++,
                    PeakPressure = peak,
                    ContactArea = area,
                    RiskScore = risk
                });

                if (index >= 500) break;
            }

            return Ok(points);
        }

        [HttpPost("frame/{index:int}")]
        public ActionResult<object> GetFrame(int index, [FromBody] ImportRequest request)
        {
            var session = ResolveSession(request.SessionId);
            if (session == null || !System.IO.File.Exists(session.Path))
                return BadRequest("Session not found.");

            int current = 0;

            foreach (var matrix in _csv.ReadFrames(session.Path))
            {
                if (current == index)
                {
                    var (peak, area, risk) = _csv.AnalyzeFrame(matrix);
                    return Ok(new
                    {
                        FrameIndex = current,
                        PeakPressure = peak,
                        ContactArea = area,
                        RiskScore = risk,
                        Matrix = matrix
                    });
                }

                current++;
            }

            return NotFound("Frame index out of range.");
        }

        // ---------- USER ENDPOINTS (use current logged-in user only) ----------

        [HttpPost("user/timeline")]
        public ActionResult<List<object>> ImportTimelineForUser()
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();

            // choose that user's "session 7" (last by DisplayName)
            var session = CsvSessions.Values
                .Where(s => s.UserId == userId)
                .OrderBy(s => s.DisplayName)
                .LastOrDefault();

            if (session == null || !System.IO.File.Exists(session.Path))
                return BadRequest("No session for user.");

            var points = new List<object>();
            int index = 0;

            foreach (var matrix in _csv.ReadFrames(session.Path))
            {
                var (peak, area, risk) = _csv.AnalyzeFrame(matrix);
                points.Add(new
                {
                    FrameIndex = index++,
                    PeakPressure = peak,
                    ContactArea = area,
                    RiskScore = risk
                });

                if (index >= 500) break;
            }

            return Ok(points);
        }

        [HttpPost("user/frame/{index:int}")]
        public ActionResult<object> GetUserFrame(int index)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();

            var session = CsvSessions.Values
                .Where(s => s.UserId == userId)
                .OrderBy(s => s.DisplayName)
                .LastOrDefault();

            if (session == null || !System.IO.File.Exists(session.Path))
                return BadRequest("No session for user.");

            int current = 0;

            foreach (var matrix in _csv.ReadFrames(session.Path))
            {
                if (current == index)
                {
                    var (peak, area, risk) = _csv.AnalyzeFrame(matrix);
                    return Ok(new
                    {
                        FrameIndex = current,
                        PeakPressure = peak,
                        ContactArea = area,
                        RiskScore = risk,
                        Matrix = matrix
                    });
                }

                current++;
            }

            return NotFound("Frame index out of range.");
        }
    }
}
