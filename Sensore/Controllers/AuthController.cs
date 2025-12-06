using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Sensore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
            public class LoginRequest
            {
                public string Username { get; set; } = string.Empty;
                public string Password { get; set; } = string.Empty;
            }

            [HttpPost("login")]
            public async Task<IActionResult> Login([FromBody] LoginRequest request)
            {
                // demo users – replace with real check later
                string? userId = request.Username switch
                {
                    "user1" when request.Password == "demo123" => "user1",
                    "user2" when request.Password == "demo123" => "user2",
                    "admin" when request.Password == "admin123" => "admin",
                    _ => null
                };

                if (userId == null) return Unauthorized();

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, userId),
                    new Claim(ClaimTypes.Name, request.Username),
                };

                if (userId == "admin")
                    claims.Add(new Claim(ClaimTypes.Role, "Admin"));
                else
                    claims.Add(new Claim(ClaimTypes.Role, "User"));

                var identity = new ClaimsIdentity(
                    claims,
                    CookieAuthenticationDefaults.AuthenticationScheme);

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(identity));

                return Ok(new { userId, role = userId == "admin" ? "Admin" : "User" });
            }

            [HttpPost("logout")]
            public async Task<IActionResult> Logout()
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return Ok();
            }
    }
}
