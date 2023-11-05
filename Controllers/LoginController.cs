using Microsoft.AspNetCore.Mvc;
using Todo.Data;
using Todo.Models;
using System;
using System.Linq;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Todo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly AppDbContext _context;

        public LoginController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                var logins = _context.Login.ToList();
                return Ok(logins);
            }
            catch (Exception ex)
            {
                return BadRequest("Não foi possível listar as contas de usuário.");
            }
        }

        [HttpPost("CreateAccount")]
        public IActionResult CreateAccount([FromBody] LoginModel model)
        {
            try
            {
                _context.Login.Add(model);
                _context.SaveChanges();
                return Ok("Conta de usuário criada com sucesso.");
            }
            catch (Exception ex)
            {
                return BadRequest("Não foi possível criar a conta de usuário.");
            }
        }

        [HttpPost("Authenticate")]
        public IActionResult Authenticate([FromBody] LoginModel model)
        {
            try
            {
                var user = _context.Login.FirstOrDefault(x => x.Username == model.Username && x.Password == model.Password);

                if (user == null)
                {
                    return BadRequest(new { message = "Usuário ou senha incorretos." });
                }

                var token = GerarTokenJwt(user);

                return Ok(new { token });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Usuário ou senha incorretos." });
            }
        }

        private string GerarTokenJwt(LoginModel user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes("abcdefghijklmnopqrsssswxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789");

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, user.Username),
                 }),
                Expires = DateTime.UtcNow.AddHours(1), 
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}
