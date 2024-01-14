using Microsoft.AspNetCore.Mvc;
using Todo.Data;
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
           TaskEntity task = context.Tasks.FirstOrDefault(x => x.Id == id);

            if (task == null) return new NotFoundResult();

            TaskEntity taskDetails = new TaskEntity()
            {
                Title = task.Title,
                Description = task.Description,
                CreatedAt = task.CreatedAt,
                Done = task.Done
            };

            return Ok(taskDetails);
        }

        public IActionResult InsertTask (TaskEntity model, int userId)
        {
            var category = context.CategorieTasks.FirstOrDefault(c => c.Id == model.CategorieTaskId);

            if(category == null || model == null) return new BadRequestResult();

            TaskEntity updateTask = new ()
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

            return Ok(updateTask);
        }

        public IActionResult EditTask (TaskEntity model, int id)
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
        public IActionResult AsignTask (TaskEntity model)
        {
            var user = context.Users.FirstOrDefault(x => x.Id == model.UserId);

            if (user == null) return new BadRequestResult();

            TaskEntity taskToAsign = new TaskEntity()
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

            return Ok(taskToAsign);
        }
    }
}
