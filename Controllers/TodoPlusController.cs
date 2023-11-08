using Microsoft.AspNetCore.Mvc;
using Todo.Data;
using Todo.Models;

namespace Todo.Controllers
{
    [ApiController]
    public class TodoPlusController : ControllerBase
    {
        [HttpGet("/TasksToDo")]
        public IActionResult Get([FromServices] AppDbContext context)
        {
            return Ok(context.TodoItemsPlus.Where(x => x.Done == false).ToList());
        }

        [HttpGet("/ListTaskDone")]
        public IActionResult ListTaskDone([FromServices] AppDbContext context)
        {
            return Ok(context.TodoItemsPlus.Where(x => x.Done == true).ToList());
        }

        [HttpGet("/GetById/{id:int}")]
        public IActionResult GetById(
            [FromRoute] int id,
            [FromServices] AppDbContext context)
        {
            TodoModelPlus task = context.TodoItemsPlus.FirstOrDefault(x => x.Id == id);

            if (task == null)
                return NotFound();

            return Ok(task);
        }

        [HttpPost("/insertTask")]
        public IActionResult Post(
        [FromBody] TodoModelPlus model,
        [FromServices] AppDbContext context)
        {
            if(model == null) return BadRequest();

            TodoModelPlus updateTask = new TodoModelPlus()
            {
                Title = model.Title,
                Description = model.Description
            };

            context.TodoItemsPlus.Add(updateTask);
            context.SaveChanges();
            return Ok(updateTask);
        }
           
        [HttpPut("/edit/{id:int}")]
        public IActionResult Put(
        [FromRoute] int id,
        [FromBody] TodoModelPlus model,
        [FromServices] AppDbContext context)
        {
            if(model == null) return BadRequest();

            var taskToEdit = context.TodoItemsPlus.FirstOrDefault(x => x.Id == id);

            taskToEdit = new TodoModelPlus() 
            {
                Title = model.Title,
                Description = model.Description
            };

            context.TodoItemsPlus.Update(taskToEdit);
            context.SaveChanges();

            return Ok(taskToEdit);
        }

        [HttpDelete("/deletar/{id:int}")]
        public IActionResult Delete(
        [FromRoute] int id,
        [FromServices] AppDbContext context)
        {
            var modelFromFrontend = context.TodoItemsPlus.FirstOrDefault(x => x.Id == id);
            if (modelFromFrontend == null)
            {
                return NotFound();
            }

            context.TodoItemsPlus.Remove(modelFromFrontend);
            context.SaveChanges();

            return Ok();
        }

        [HttpPut("/done/{id:int}")]
        public IActionResult Done(
        [FromRoute] int id,
        [FromServices] AppDbContext context)
        {
            var model = context.TodoItemsPlus.FirstOrDefault(x => x.Id == id);
            if (model == null)
            {
                return NotFound();
            }

            model.Done = !model.Done;

            context.TodoItemsPlus.Update(model);
            context.SaveChanges();

            return Ok(model);
        }

    }
}