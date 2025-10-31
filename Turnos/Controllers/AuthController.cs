using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using Turnos.data.entidades;
using System.Text;
using System.Configuration;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using Turnos.data.DTOs.Authentication;
using Turnos.core.servicios;
using Turnos.core.interfaces;

namespace Turnos.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IAuthService _authService;
        private readonly IConfiguration _configuration;
        private readonly RoleManager<IdentityRole> _roleManager;
        public AuthController(UserManager<IdentityUser> userManager, IConfiguration configuration, 
            IAuthService authService, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _authService = authService;
            _configuration = configuration;
            _roleManager = roleManager;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = new IdentityUser
            {
                UserName = request.UserName,
                Email = request.Email
            };

            var result = await _authService.RegisterAsync(user, request.Password,request.Role);

            if (!result.Succeeded) return BadRequest(result.Errors);

            return Ok("Usuario registrado correctamente");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto request)
        {
            var result = await _authService.LoginAsync(request);
            if (result == null)
                return Unauthorized("Credenciales invalidas");

            return Ok(result);
        }

        [HttpPost("create-role")]
        public async Task<IActionResult> CreateRole(string roleName)
        {
            if (string.IsNullOrWhiteSpace(roleName))
                return BadRequest("El nombre del rol no puede estar vacío");
            if (await _roleManager.RoleExistsAsync(roleName))
                return BadRequest("El rol ya existe");

            await _roleManager.CreateAsync(new IdentityRole(roleName));
            return Ok($"Rol '{roleName}' creado correctamente");
        }
        
    }
}
