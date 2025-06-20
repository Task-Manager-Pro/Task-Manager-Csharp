using Microsoft.AspNetCore.Mvc;
using Todo.Data;
using Todo.Domain;
using Todo.Models;
using Todo.Dal;

namespace Todo.Services
{
    public class TaskManagerServices: ControllerBase
    {
        private readonly AppDbContext context;
        private readonly TaskDal _taskDal;

        public TaskManagerServices(AppDbContext dbContext, TaskDal taskDal)
        {
            context = dbContext;
            _taskDal = taskDal;
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

                var newId = _taskDal.InsertTask(newTask);

                return Ok(new { taskId = newId });
            }
            catch (Exception ex)
            {
                
                Console.WriteLine($"Erro ao inserir tarefa: {ex}");

                return StatusCode(500, "Ocorreu um erro ao criar a tarefa.");
            }
        }

        public IActionResult EditTask (TaskModel model, int id)
        {
            var taskToEdit = context.Tasks.FirstOrDefault(x => x.Id == id);

            if (taskToEdit == null)
            {
                return new NotFoundResult();
            }


            taskToEdit.Title = model.Title;
            taskToEdit.Description = model.Description;

            _taskDal.UpdateTask(taskToEdit);

            return Ok(taskToEdit);
        }
        public IActionResult DeleteTask (int id)
        {
            TaskEntity taskToDelete = context.Tasks.FirstOrDefault(x => x.Id == id);

            if (taskToDelete == null) return new NotFoundResult();

            _taskDal.DeleteTask(taskToDelete);

            return Ok();
        }
        public IActionResult DoneTask (int id)
        {
            TaskEntity task = context.Tasks.FirstOrDefault(x => x.Id == id);

            if (task == null) return new BadRequestResult();

            var updated = _taskDal.ToggleDone(id);

            return updated == null ? BadRequest() : Ok(updated);
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

            _taskDal.InsertTask(taskToAsign);

            return Ok(taskToAsign);
        }
    }
}
