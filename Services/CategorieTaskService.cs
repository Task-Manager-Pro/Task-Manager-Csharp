using Microsoft.AspNetCore.Mvc;
using Todo.Data;

namespace Todo.Services
{
    public class CategorieTaskService
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
                return new OkObjectResult("Não foi possível listar as categorias de tarefas.");
            }
        }

    }
}
