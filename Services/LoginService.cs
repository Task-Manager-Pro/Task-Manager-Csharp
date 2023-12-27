using Microsoft.AspNetCore.Mvc;
using Todo.Data;
using Todo.Models;

namespace Todo.Services
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginService
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
    }
}
