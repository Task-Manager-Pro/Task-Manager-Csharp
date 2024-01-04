using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Todo.Data;
using Todo.Models;


namespace Todo.Services
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginService: ControllerBase
    {
        private readonly AppDbContext _context;

        public LoginService(AppDbContext context)
        {
            _context = context;
        }  
        [HttpGet]
        public IActionResult CreateAccount([FromBody] UserEntity model)
        {
            if (model != null)
            {
                _context.Users.Add(model);
                _context.SaveChanges();
                return new ObjectResult("Conta de usuário criada com sucesso.");
            }
            else
            {
                return new ObjectResult("Não foi possível criar a conta de usuário.");
            }
        }

        public IActionResult Authenticate([FromBody] UserEntity model)
        {
            var user = _context.Users.FirstOrDefault(x => x.Username == model.Username && x.Password == model.Password);

            if (user == null)
            {
                return new ObjectResult("Usuário ou senha inválidos.");
            }

            var token = GerarTokenJwt(user);

            return Ok(new
            {
                token,
                user = new
                {
                    Id = user.Id,
                    Username = user.Username,
                    IsAdmin = user.IsAdmin,
                    IsLogged = true 
                }
            });
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
