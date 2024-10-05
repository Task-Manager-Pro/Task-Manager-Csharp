using Microsoft.AspNetCore.Mvc;
using Todo.Data;
using Todo.Domain;
using Todo.Models;

namespace Todo.Services
{
    public class TaskManagerServices: ControllerBase
    {
        private readonly AppDbContext context;

        public TaskManagerServices(AppDbContext dbContext)
        {
            context = dbContext;
        }

        public IActionResult GetTasksToDo()
        {
            var tasks = context.Tasks
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
            return Ok(tasks);
        }

        public IActionResult GetTaskDone()
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

        public IActionResult GetAllTasks()
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

        public IActionResult GetTasksByUser (int userId)
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

        public IActionResult GetById(int id)
        {
           TaskModel task = context.Tasks.FirstOrDefault(x => x.Id == id);

            if (task == null) return new NotFoundResult();

            TaskModel taskDetails = new TaskModel()
            {
                Title = task.Title,
                Description = task.Description,
                CreatedAt = task.CreatedAt,
                Done = task.Done
            };

            return Ok(taskDetails);
        }

        public IActionResult InsertTask (TaskModel model, int userId)
        {
            try
            {
                var category = context.CategorieTasks.FirstOrDefault(c => c.Id == model.CategorieTaskId);

                if (category == null || model == null)
                {
                    return BadRequest("Categoria ou modelo inválido.");
                }

                TaskEntity newTask = new TaskEntity
                {
                    Title = model.Title,
                    Description = model.Description,
                    Done = false,
                    CreatedAt = DateTime.Now,
                    CategorieTaskId = model.CategorieTaskId,
                    Category = category,
                    UserId = userId
                };

                context.Tasks.Add(newTask);
                context.SaveChanges();

                return Ok(new { taskId = newTask.Id });
            }
            catch (Exception ex)
            {
                
                Console.WriteLine($"Erro ao inserir tarefa: {ex}");

                return StatusCode(500, "Ocorreu um erro ao criar a tarefa.");
            }
        }

        public IActionResult EditTask (TaskModel model, int id)
        {
            TaskEntity taskToEdit = context.Tasks.FirstOrDefault(x => x.Id == id);

            if (taskToEdit == null) return new NotFoundResult();

            taskToEdit = new TaskEntity()
            {
                Title = model.Title,
                Description = model.Description
            };

            context.Tasks.Update(taskToEdit);
            context.SaveChanges();

            return Ok(taskToEdit);
        } 
        public IActionResult DeleteTask (int id)
        {
            TaskEntity taskToDelete = context.Tasks.FirstOrDefault(x => x.Id == id);

            if (taskToDelete == null) return new NotFoundResult();

            context.Tasks.Remove(taskToDelete);
            context.SaveChanges();

            return Ok();
        }
        public IActionResult DoneTask (int id)
        {
            TaskEntity task = context.Tasks.FirstOrDefault(x => x.Id == id);

            if (task == null) return new BadRequestResult();

            task.Done = !task.Done;

            context.Tasks.Update(task);
            context.SaveChanges();

            return Ok(task);
        }
        public IActionResult AsignTask (TaskModel model)
        {
            var user = context.Users.FirstOrDefault(x => x.Id == model.UserId);
            var category = context.CategorieTasks.FirstOrDefault(x => x.Id == model.CategorieTaskId);

            if (user == null) return new BadRequestResult();

            TaskEntity taskToAsign = new TaskEntity()
            {
                Title = model.Title,
                Description = model.Description,
                Done = false,
                CreatedAt = DateTime.Now,
                CategorieTaskId = model.CategorieTaskId,
                Category = category,
                UserId = model.UserId
            };

            context.Tasks.Add(taskToAsign);
            context.SaveChanges();

            return Ok(taskToAsign);
        }
    }
}
