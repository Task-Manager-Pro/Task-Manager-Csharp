using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Todo.Data;
using Todo.Migrations;
using Todo.Models;
using Todo.Services;

namespace Todo.Controllers
{
    [ApiController]
    public class TaskManagerController : ControllerBase
    {
        private readonly TaskManagerServices taskManagerServices;

        [HttpGet("/TasksToDo")]
        public IActionResult Get()
        {
            try
            {
                var tasksToDo = taskManagerServices.GetTasksToDo();
                return Ok(tasksToDo);
            }
            catch (System.Exception)
            {
                return BadRequest();
            }
        }

        [HttpGet("/ListTaskDone")]
        public IActionResult ListTaskDone()
        {
           try
           {
                var tasksDone = taskManagerServices.GetTaskDone();
                return Ok(tasksDone);
           }catch(System.Exception)
            {
               return BadRequest();
           }
        }

        [HttpGet("/ListllTasks")]
        public IActionResult ListAllTasks()
        {
            try
            {
                var allTasks = taskManagerServices.GetAllTasks();
                return Ok(allTasks);
            }catch(System.Exception) 
            { 
                return BadRequest();
            }
        }

        [HttpGet("ListTaskByUser/{userId}")]
        public IActionResult ListTarefaByUser(
        [FromRoute] int userId)
        {
            try
            {
                var tasksByUser = taskManagerServices.GetTasksByUser(userId);
                return Ok(tasksByUser);
            }catch(System.Exception)
            {
                return BadRequest();
            }
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