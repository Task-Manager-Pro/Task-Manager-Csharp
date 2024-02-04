using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Todo.Data;
using Todo.Models;
using Todo.Services;

namespace Todo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategorieTaskManagerController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly CategorieTaskService _categorieTaskService;

        public CategorieTaskManagerController(AppDbContext context)
        {
            _context = context;
        }

        [Authorize]
        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                var categorieTasks = _categorieTaskService.Get();
                return Ok(categorieTasks);
            }
            catch (Exception ex)
            {
                return BadRequest("Não foi possível listar as categorias de tarefas.");
            }
        }

        [Authorize]
        [HttpPost("CreateCategorieTask")]
        public IActionResult CreateCategorieTask([FromBody] CategorieTaskEntity model)
        {
            try
            {
               var categorieTask = _categorieTaskService.CreateCategorieTask(model);
                return Ok(categorieTask);
            }
            catch (Exception ex)
            {
                return BadRequest("Não foi possível criar a categoria de tarefa.");
            }
        }

        [Authorize]
        [HttpPut("UpdateCategorieTask")]
        public IActionResult UpdateCategorieTask([FromBody] CategorieTaskEntity model)
        {
            try
            {
               var categorieTask = _categorieTaskService.UpdateCategorieTask(model);
                return Ok(categorieTask);
            }
            catch (Exception ex)
            {
                return BadRequest("Não foi possível atualizar a categoria de tarefa.");
            }
        }

        [Authorize]
        [HttpDelete("DeleteCategorieTask/{id}")]
        public IActionResult DeleteCategorieTask(int id)
        {
            try
            {
                var categorieTask = _context.CategorieTasks.FirstOrDefault(x => x.Id == id);
                _context.CategorieTasks.Remove(categorieTask);
                _context.SaveChanges();
                return Ok("Categoria de tarefa removida com sucesso.");
            }
            catch (Exception ex)
            {
                return BadRequest("Não foi possível remover a categoria de tarefa.");
            }
        }   
    }

}