using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Task.API.Entities;

namespace Task.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public AuthController(IConfiguration configuration)
        {

            _configuration = configuration;
        }

        private Users AuthenticateUser(Users user)
        {
            Users _user = null;

            if (user.Username == "admin" && user.Password == "123456")
            {
                _user = new Users { Username = "Codid" };
            }

            return _user;
        }

        private string GenerateToken(Users users)
        {

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

            var token = new JwtSecurityToken(_configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"], null,
                expires: DateTime.Now.AddDays(30), signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);

        }

        [AllowAnonymous]
        [HttpPost("Login")]
        public IActionResult Login(Users user)
        {
            IActionResult response = Unauthorized();
            var authUser = AuthenticateUser(user);
            if (authUser != null)
            {
                var token = GenerateToken(authUser);
                response = Ok(new { Token = token });
            }
            return response;
        }
    }
}
