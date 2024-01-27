using Microsoft.AspNetCore.Mvc;
using Todo.Data;
using Todo.Models;
using Todo.Services;

namespace Todo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : ControllerBase
    {
        
        private readonly LoginService _loginService;

        public LoginController(LoginService loginService)
        {
            _loginService = loginService;
        }

        [HttpGet("ListUsers")]
        public IActionResult ListUsers()
        {
            try
            {
                var listUsers = _loginService.ListUsers();
                return Ok(listUsers);
            }
            catch (Exception ex)
            {
                return BadRequest("Não foi possível listar os usuários.");
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
               var autenticateService = _loginService.Authenticate(model);
                return Ok(autenticateService);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Usuário ou senha incorretos." });
            }
        }      
    }
}
