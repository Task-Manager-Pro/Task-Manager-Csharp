using Microsoft.AspNetCore.Mvc;
using Todo.Data;
using Todo.Models;

namespace Todo.Controllers{
    [ApiController]
    public class HomeController : ControllerBase
    {
        [HttpGet("/")]
        public List<TodoModel> Get([FromServices] AppDbContext context)
        {
            return context.TodoItems.ToList();
        }

        [HttpPost("/")]
        public TodoModel Post(
        [FromBody] TodoModel model,
        [FromServices] AppDbContext context)
        {
            context.TodoItems.Add(model);
            context.SaveChanges();
            return model;
        }
    }
}