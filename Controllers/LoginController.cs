using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Todo.Domain;
using Todo.Interfaces;
using Todo.Services;

namespace Todo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly ILoginService _loginService;

        public LoginController(ILoginService loginService)
        {
            _loginService = loginService;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("ListUsers")]
        public async Task<IActionResult> ListUsers(CancellationToken ct)
        {
            try
            {
                var users = await _loginService.ListUsersAsync(ct);
                return Ok(users);
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Não foi possível listar os usuários." });
            }
        }

        [HttpPost("CreateAccount")]
        [AllowAnonymous]
        public async Task<IActionResult> CreateAccount([FromBody] UserEntity model, CancellationToken ct)
        {
            if (model == null)
                return BadRequest(new { message = "Dados inválidos." });

            try
            {
                var created = await _loginService.CreateAccountAsync(model, ct);

                if (!created)
                    return BadRequest(new { message = "Não foi possível criar a conta de usuário." });

                return Ok(new { message = "Conta de usuário criada com sucesso." });
            }
            catch (InvalidOperationException ex)
            {
                // Tratamento de duplicidade de username
                return Conflict(new { message = ex.Message });
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Erro inesperado ao criar conta." });
            }
        }

        [HttpPost("Authenticate")]
        [AllowAnonymous]
        public async Task<IActionResult> Authenticate([FromBody] UserEntity model, CancellationToken ct)
        {
            if (model is null || string.IsNullOrWhiteSpace(model.Username) || string.IsNullOrWhiteSpace(model.Password))
                return BadRequest(new { message = "Informe usuário e senha." });

            var result = await _loginService.AuthenticateAsync(model.Username, model.Password, ct);

            if (!result.Succeeded)
                return Unauthorized(new { message = "Usuário ou senha incorretos." });

            return Ok(new
            {
                token = result.Token,
                user = result.User
            });
        }
    }
}
