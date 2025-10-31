using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Turnos.core.interfaces;
using Turnos.data.DTOs.Authentication;
using Turnos.data.entidades;

namespace Turnos.core.servicios
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;

        public AuthService(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, RoleManager<IdentityRole> roleManager, IConfiguration configuracion)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuracion;
            _signInManager = signInManager;
        }

        //Login

        public async Task<AuthResponseDto?> LoginAsync(LoginDto request)
        {
            //buscar usuario por email
            var user = await _userManager.FindByEmailAsync(request.EmailOrUserName);
            if (user == null) return null;

            //verificar password
            var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
            if (!result.Succeeded) return null;

            //obtener roles del usuario
            var roles = await _userManager.GetRolesAsync(user);

            //si se verifico el email y password, generar el token
            var token = await GenerateJwtToken(user, roles);

            //Respuesta del login
            return new AuthResponseDto
            {
                Token = token.Token,
                Expiration = token.Expiration,
                UserName = user.UserName!,
                Email = user.Email!,
                Roles = roles.ToList()
            };
        }

        //Register
        public async Task<IdentityResult> RegisterAsync(IdentityUser usuario, string password, string role)
        {
            var roleExists = await _roleManager.RoleExistsAsync(role);
            if (!roleExists)
            {
                return IdentityResult.Failed(new IdentityError
                {
                    Code = "RoleNotFound",
                    Description = $"El rol '{role}' no existe."
                });
            }

            var result = await _userManager.CreateAsync(usuario, password);
            if (!result.Succeeded) return result;

            await _userManager.AddToRoleAsync(usuario, role);

            return result;
        }

        private async Task<JwtResult> GenerateJwtToken(IdentityUser user, IList<string> roles)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.RsaSha256);

            //tiempo de expiracion desde config
            var expiration = DateTime.UtcNow.AddMinutes(jwtSettings.GetValue<double>("ExpireMinutes"));
            
            //creamos el claim
            var claims = new List<Claim> 
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email!),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName!),
                new Claim("uid", user.Id)
            };
            //agregar roles al claim
            foreach(var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            //se crea el token
            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: expiration,
                signingCredentials: creds
            );

            return new JwtResult
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiration = expiration
            };
        }
    }
}
