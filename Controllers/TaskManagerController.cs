using Microsoft.AspNetCore.Authorization;
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
          
            var tasksDone = context.Tasks
                .Where(x => x.Done == false)
                .Select(task => new
                {
                    TaskId = task.Id,
                    TaskTitle = task.Title,
                    TaskDescription = task.Description,
                    Done = task.Done,
                    CreatedAt = task.CreatedAt,
                    CategoryName = context.CategorieTasks
                        .Where(category => category.Id == task.CategorieTaskId)
                        .Select(category => category.Name)
                        .FirstOrDefault()
                })
                .ToList();  
            return Ok(tasksDone);
        }

        [HttpGet("/ListTaskDone")]
        public IActionResult ListTaskDone([FromServices] AppDbContext context)
        {
            var tasks = context.Tasks
                .Where(x => x.Done == true)
                .Select(task => new
                {
                    TaskId = task.Id,
                    TaskTitle = task.Title,
                    TaskDescription = task.Description,
                    Done = task.Done,
                    CreatedAt = task.CreatedAt,
                    CategoryName = context.CategorieTasks
                        .Where(category => category.Id == task.CategorieTaskId)
                        .Select(category => category.Name)
                        .FirstOrDefault()
                })
                .ToList();

            return Ok(tasks);
        }

        [HttpGet("/ListllTasks")]
        public IActionResult ListAllTasks([FromServices] AppDbContext context)
        {
            var tasks = context.Tasks
                .Select(task => new
                {
                    TaskId = task.Id,
                    TaskTitle = task.Title,
                    TaskDescription = task.Description,
                    Done = task.Done,
                    CreatedAt = task.CreatedAt,
                    CategoryName = context.CategorieTasks
                        .Where(category => category.Id == task.CategorieTaskId)
                        .Select(category => category.Name)
                        .FirstOrDefault()
                }) 
                .ToList();

            return Ok(tasks);
        }

        [HttpGet("ListTaskByUser/{userId}")]
        public IActionResult ListTarefaByUser(
        [FromRoute] int userId,
        [FromServices] AppDbContext context)
        {
            var tasks = context.Tasks
                .Where(x => x.UserId == userId)
                .Select(task => new
                {
                    TaskId = task.Id,
                    TaskTitle = task.Title,
                    TaskDescription = task.Description,
                    Done = task.Done,
                    CreatedAt = task.CreatedAt,
                    CategoryName = context.CategorieTasks
                        .Where(category => category.Id == task.CategorieTaskId)
                        .Select(category => category.Name)
                        .FirstOrDefault()
                })
                .ToList();

            return Ok(tasks);
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

        [HttpPost("/insertTask/{userId}")]
        public IActionResult Post(
        [FromBody] TaskEntity model,
        [FromRoute] int userId,
        [FromServices] AppDbContext context)
        {
            var category = context.CategorieTasks.FirstOrDefault(c => c.Id == model.CategorieTaskId);

            if(category == null || model == null) return BadRequest();

            TaskEntity updateTask = new TaskEntity()
            {
                Title = model.Title,
                Description = model.Description,
                Done = false,
                CreatedAt = DateTime.Now,
                CategorieTaskId = model.CategorieTaskId,
                Category = context.CategorieTasks.FirstOrDefault(c => c.Id == model.CategorieTaskId),
                UserId = userId
            };

            context.Tasks.Add(updateTask);
            context.SaveChanges();
            return Ok(new { Message = "Tarefa criada com sucesso." });
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

        [HttpPost("/asignTask/{userId:int}")]
        public IActionResult AsignTask(
            [FromBody] TaskEntity model,
            [FromRoute] int userId,
            [FromServices] AppDbContext context)
        {
            var user = context.Users.FirstOrDefault(x => x.Id == userId);

            if (user == null) return BadRequest();

           var taskToAsign = new TaskEntity()
           {
                Title = model.Title,
                Description = model.Description,
                Done = false,
                CreatedAt = DateTime.Now,
                CategorieTaskId = model.CategorieTaskId,
                Category = context.CategorieTasks.FirstOrDefault(c => c.Id == model.CategorieTaskId),
                UserId = model.UserId
           };

            context.Tasks.Add(taskToAsign);
            context.SaveChanges();

            return Ok(new { Message = "Tarefa designada com sucesso.", taskToAsign });
        }

    }
}