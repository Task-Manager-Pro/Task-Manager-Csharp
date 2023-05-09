using Microsoft.AspNetCore.Mvc;
using Todo.Data;
using Todo.Models;

namespace Todo.Controllers{
    [ApiController]
    public class HomeController : ControllerBase
    {
        [HttpGet("/")]
        public IActionResult Get([FromServices] AppDbContext context)
        {
            return Ok(context.TodoItems.ToList());
        }

         [HttpGet("/{id:int}")]
        public IActionResult GetById(
            [FromRoute] int id,
            [FromServices] AppDbContext context)
        {
            var todos = context.TodoItems.FirstOrDefault(x => x.Id == id);
            if (todos == null)
                return NotFound();

            return Ok(todos);
        }

        [HttpPost("/")]
        public IActionResult Post(
        [FromBody] TodoModel model,
        [FromServices] AppDbContext context)
        {
            context.TodoItems.Add(model);
            context.SaveChanges();
            return Created($"/{model.Id}", model);
        }

          [HttpPut("/{id:int}")]
        public IActionResult Put(
        [FromRoute] int id,
        [FromBody] TodoModel todo,
        [FromServices] AppDbContext context)
        {
            var model = context.TodoItems.FirstOrDefault(x => x.Id == id);
            if (model == null)
            {
                return NotFound();
            }

           model.Title = todo.Title;
           model.Done =todo.Done;

            context.TodoItems.Update(model);
            context.SaveChanges();

            return Ok(model);
        }

          [HttpDelete("/{id:int}")]
        public IActionResult Delete(
            [FromRoute] int id,
        [FromServices] AppDbContext context)
        {
            var modelFromFrontend = context.TodoItems.FirstOrDefault(x => x.Id == id);
            if (modelFromFrontend == null)
                return NotFound();
            context.TodoItems.Remove(modelFromFrontend);
            context.SaveChanges();

            return Ok(modelFromFrontend);
        }
    }
}