using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Mvc;
using RapGame.API.Services;
using RapGame.Shared.Auth;

namespace RapGame.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public ActionResult<object> Login([FromBody] LoginRequest login)
        {
            var token = _authService.Autenticar(login);
            if (token == null)
                return Unauthorized(new { message = "Credenciais inválidas" });

            return Ok(new { token });
        }
    }
}