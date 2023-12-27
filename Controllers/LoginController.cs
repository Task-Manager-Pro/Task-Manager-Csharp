using Microsoft.AspNetCore.Mvc;
using Todo.Data;
using Todo.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Todo.Services;

namespace Todo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly LoginService _loginService;

        public LoginController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                var logins = _context.Users.ToList();
                return Ok(logins);
            }
            catch (Exception ex)
            {
                return BadRequest("Não foi possível listar as contas de usuário.");
            }
        }

        [HttpPost("CreateAccount")]
        public IActionResult CreateAccount([FromBody] UserEntity model)
        {
            try
            {
                var createUser = _loginService.CreateAccount(model);
                return Ok("Conta de usuário criada com sucesso.");
            }
            catch (Exception ex)
            {
                return BadRequest("Não foi possível criar a conta de usuário.");
            }
        }

        [HttpPost("Authenticate")]
        public IActionResult Authenticate([FromBody] UserEntity model)
        {
            try
            {
                var user = _context.Users.FirstOrDefault(x => x.Username == model.Username && x.Password == model.Password);

                if (user == null)
                {
                    return BadRequest(new { message = "Usuário ou senha incorretos." });
                }

                var token = GerarTokenJwt(user);

                return Ok(new
                {
                   token,
                   user = new UserEntity
                   {
                       Id = user.Id,
                       Username = user.Username,
                       IsAdmin = user.IsAdmin,
                       isLogged = true
                   }
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Usuário ou senha incorretos." });
            }
        }

        private string GerarTokenJwt(UserEntity user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(Settings.Secret);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Role, user.IsAdmin.ToString())
                 }),
                Expires = DateTime.UtcNow.AddMinutes(5), 
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}
