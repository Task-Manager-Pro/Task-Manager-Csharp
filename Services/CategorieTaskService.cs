using Microsoft.AspNetCore.Mvc;
using Todo.Data;
using Todo.Models;

namespace Todo.Services
{
    public class CategorieTaskService:ControllerBase
    {
        public readonly AppDbContext _context;

        public CategorieTaskService(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Get()
        {
            try
            {
                var categorieTasks = _context.CategorieTasks.ToList();
                return new OkObjectResult(categorieTasks);
            }
            catch (Exception ex)
            {
                return Ok("Não foi possível listar as categorias de tarefas.");
            }
        }

        public IActionResult CreateCategorieTask([FromBody] CategorieTaskEntity model)
        {
            try
            {
                CategorieTaskEntity categorieTask = new()
                {
                    Name = model.Name,
                    Description = model.Description
                };

                _context.CategorieTasks.Add(categorieTask);
                _context.SaveChanges();
                return Ok("Categoria de tarefa criada com sucesso.");
            }
            catch (Exception ex)
            {
                return BadRequest("Não foi possível criar a categoria de tarefa.");
            }
        }

    }
}
