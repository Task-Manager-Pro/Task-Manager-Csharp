using Microsoft.AspNetCore.Mvc;
using Todo.Data;

namespace Todo.Services
{
    public class TaskManagerServices
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
            return new OkObjectResult(tasks);
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
            return new OkObjectResult(tasks);
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
            return new OkObjectResult(tasks);
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
            return new OkObjectResult(tasks);
        }
    }
}
