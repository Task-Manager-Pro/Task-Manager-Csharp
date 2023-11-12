using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Todo.Data;
using Todo.Models;

namespace Todo.Controllers
{
    [ApiController]
    public class TaskManagerController : ControllerBase
    {
        [HttpGet("/TasksToDo")]
        public IActionResult Get([FromServices] AppDbContext context)
        {
            return Ok(context.Tasks.Where(x => x.Done == false).ToList());
        }

        [HttpGet("/ListTaskDone")]
        public IActionResult ListTaskDone([FromServices] AppDbContext context)
        {
            return Ok(context.Tasks.Where(x => x.Done == true).ToList());
        }

        [HttpGet("/GetById/{id:int}")]
        public IActionResult GetById(
            [FromRoute] int id,
            [FromServices] AppDbContext context)
        {
            TaskEntity task = context.Tasks.FirstOrDefault(x => x.Id == id);

            if (task == null) return NotFound();

            TaskEntity taskDetails = new TaskEntity()
            {
                Title = task.Title,
                Description = task.Description,
                CreatedAt = task.CreatedAt,
                Done = task.Done
            };

            return Ok(taskDetails);
        }

        [HttpPost("/insertTask/{categoryId}/{userId}")]
        public IActionResult Post(
        [FromBody] TaskEntity model,
        [FromRoute] int categoryId,
        [FromRoute] int userId,
        [FromServices] AppDbContext context)
        {
            var category = context.CategorieTasks.FirstOrDefault(c => c.Id == categoryId);

            if(category == null || model == null) return BadRequest();

            TaskEntity updateTask = new TaskEntity()
            {
                Title = model.Title,
                Description = model.Description,
                Done = false,
                CreatedAt = DateTime.Now,
                CategorieTaskId = categoryId,
                UserId = userId
            };

            context.Tasks.Add(updateTask);
            context.SaveChanges();
            return Ok("Tarefa criada com sucesso.");
        }
           
        [HttpPut("/edit/{id:int}")]
        public IActionResult Put(
        [FromRoute] int id,
        [FromBody] TaskEntity model,
        [FromServices] AppDbContext context)
        {
            if(model == null) return BadRequest();

            var taskToEdit = context.Tasks.FirstOrDefault(x => x.Id == id);

            taskToEdit = new TaskEntity() 
            {
                Title = model.Title,
                Description = model.Description
            };

            context.Tasks.Update(taskToEdit);
            context.SaveChanges();

            return Ok(taskToEdit);
        }

        [HttpDelete("/delete/{id:int}")]
        public IActionResult Delete(
        [FromRoute] int id,
        [FromServices] AppDbContext context)
        {
            TaskEntity taskToDelete = context.Tasks.FirstOrDefault(x => x.Id == id);

            if (taskToDelete == null) return NotFound();

            context.Tasks.Remove(taskToDelete);
            context.SaveChanges();

            return Ok();
        }

        [HttpPut("/done/{id:int}")]
        public IActionResult Done(
        [FromRoute] int id,
        [FromServices] AppDbContext context)
        {
            var task = context.Tasks.FirstOrDefault(x => x.Id == id);

            if (task == null) return BadRequest();

            task.Done = !task.Done;

            context.Tasks.Update(task);
            context.SaveChanges();

            return Ok(task);
        }

    }
}