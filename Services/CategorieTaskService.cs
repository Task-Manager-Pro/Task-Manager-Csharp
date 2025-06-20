using Microsoft.AspNetCore.Mvc;
using Todo.Data;
using Todo.Domain;
using Todo.Models;
using Todo.Dal;

namespace Todo.Services
{
    public class CategorieTaskService:ControllerBase
    {
        public readonly AppDbContext _context;
        private readonly CategorieTaskDal _dal;

        public CategorieTaskService(AppDbContext context, CategorieTaskDal dal)
        {
            _context = context;
            _dal = dal;
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

        public IActionResult CreateCategorieTask([FromBody] CategorieTaskModel model)
        {
            try
            {
                CategorieTaskEntity categorieTask = new()
                {
                    Name = model.Name,
                    Description = model.Description
                };

                _dal.AddCategorieTask(categorieTask);
                return Ok("Categoria de tarefa criada com sucesso.");
            }
            catch (Exception ex)
            {
                return BadRequest("Não foi possível criar a categoria de tarefa.");
            }
        }

        public IActionResult UpdateCategorieTask([FromBody] CategorieTaskModel model)
        {
            try
            {
                var categorieToUpdate = _context.CategorieTasks.FirstOrDefault(c => c.Id == model.Id);
                if (categorieToUpdate == null)
                {
                    return NotFound();
                }

                categorieToUpdate.Name = model.Name;
                categorieToUpdate.Description = model.Description;

                _dal.UpdateCategorieTask(categorieToUpdate);
                return Ok("Categoria de tarefa atualizada com sucesso.");
            }
            catch (Exception ex)
            {
                return BadRequest("Não foi possível atualizar a categoria de tarefa.");
            }
        }

    }
}
