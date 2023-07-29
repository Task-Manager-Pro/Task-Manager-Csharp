using Microsoft.AspNetCore.Mvc;
using Todo.Data;
using Todo.Models;
using System;
using System.Linq;

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

        [HttpPost("/CreateAccount")]
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

      [HttpPost("/Authenticate")]
        public IActionResult Authenticate([FromBody] LoginModel model)
        {
            try
            {
                var user = _context.Login.FirstOrDefault(x => x.Username == model.Username && x.Password == model.Password);

                if (user == null)
                {
                    return BadRequest(new { message = "Usuário ou senha incorretos." });
                }

                return Ok(new { message = "Usuário logado com sucesso." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Não foi possível autenticar o usuário." });
            }
        }

    }
}
