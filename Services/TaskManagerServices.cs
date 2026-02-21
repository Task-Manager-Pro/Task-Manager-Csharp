using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Todo.Data;
using Todo.Domain;
using Todo.Models;
using Todo.Dal;

namespace Todo.Services
{
    public class TaskManagerServices : ControllerBase
    {
        private readonly AppDbContext context;
        private readonly TaskDal _taskDal;
        private readonly ILogger<TaskManagerServices> _logger;

        public TaskManagerServices(AppDbContext dbContext, TaskDal taskDal, ILogger<TaskManagerServices> logger)
        {
            context = dbContext;
            _taskDal = taskDal;
            _logger = logger;
        }

        public IActionResult GetTasksToDo()
        {
            var tasks = context.Tasks
                .Where(x => x.Done == false)
                .Select(MapLegacyList())
                .ToList();
            return Ok(tasks);
        }

        public IActionResult GetTaskDone()
        {
            var tasks = context.Tasks
                .Where(x => x.Done)
                .Select(MapLegacyList())
                .ToList();
            return Ok(tasks);
        }

        public IActionResult GetAllTasks()
        {
            var tasks = context.Tasks
                .Select(MapLegacyList())
                .ToList();
            return Ok(tasks);
        }

        public IActionResult GetTasksByUser(int userId)
        {
            var tasks = context.Tasks
                .Where(x => x.UserId == userId)
                .Select(MapLegacyList())
                .ToList();
            return Ok(tasks);
        }

        public IActionResult GetById(int id)
        {
            TaskEntity? task = context.Tasks.FirstOrDefault(x => x.Id == id);

            if (task == null) return new NotFoundResult();

            TaskModel taskDetails = new()
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                CreatedAt = task.CreatedAt,
                Done = task.Done,
                CategorieTaskId = task.CategorieTaskId,
                EstimateMinutes = task.EstimateMinutes,
                SpentMinutes = task.SpentMinutes,
                DueDate = task.DueDate,
                RowVersion = Convert.ToBase64String(task.RowVersion)
            };

            return Ok(taskDetails);
        }

        public IActionResult InsertTask(TaskModel model, int userId)
        {
            try
            {
                var category = context.CategorieTasks.FirstOrDefault(c => c.Id == model.CategorieTaskId);

                if (category == null || model == null)
                {
                    return BadRequest("Categoria ou modelo inválido.");
                }

                var user = context.Users.FirstOrDefault(u => u.Id == userId);
                var maxOrder = context.Tasks
                    .Where(t => t.CategorieTaskId == model.CategorieTaskId)
                    .Select(t => (int?)t.Order)
                    .Max() ?? -1;

                TaskEntity newTask = new()
                {
                    Title = model.Title,
                    Description = model.Description,
                    Done = false,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    UpdatedBy = userId,
                    CategorieTaskId = model.CategorieTaskId,
                    Category = category,
                    UserId = userId,
                    TenantId = user?.TenantId ?? 1,
                    Order = maxOrder + 1,
                    State = TaskState.Todo,
                    EstimateMinutes = model.EstimateMinutes,
                    SpentMinutes = model.SpentMinutes,
                    DueDate = model.DueDate
                };

                var newId = _taskDal.InsertTask(newTask);

                return Ok(new { taskId = newId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao inserir tarefa");
                return StatusCode(500, "Ocorreu um erro ao criar a tarefa.");
            }
        }

        public IActionResult EditTask(TaskModel model, int id)
        {
            var taskToEdit = context.Tasks.FirstOrDefault(x => x.Id == id);

            if (taskToEdit == null)
            {
                return new NotFoundResult();
            }

            taskToEdit.Title = model.Title;
            taskToEdit.Description = model.Description;
            taskToEdit.UpdatedAt = DateTime.UtcNow;
            taskToEdit.UpdatedBy = model.UserId;

            _taskDal.UpdateTask(taskToEdit);

            return Ok(taskToEdit);
        }

        public IActionResult DeleteTask(int id)
        {
            TaskEntity? taskToDelete = context.Tasks.FirstOrDefault(x => x.Id == id);

            if (taskToDelete == null) return new NotFoundResult();

            _taskDal.DeleteTask(taskToDelete);

            return Ok();
        }

        public IActionResult DoneTask(int id)
        {
            TaskEntity? task = context.Tasks.FirstOrDefault(x => x.Id == id);

            if (task == null) return new BadRequestResult();

            var updated = _taskDal.ToggleDone(id);

            return updated == null ? BadRequest() : Ok(updated);
        }

        public IActionResult AsignTask(TaskModel model)
        {
            var user = context.Users.FirstOrDefault(x => x.Id == model.UserId);
            var category = context.CategorieTasks.FirstOrDefault(x => x.Id == model.CategorieTaskId);

            if (user == null) return new BadRequestResult();

            TaskEntity taskToAsign = new()
            {
                Title = model.Title,
                Description = model.Description,
                Done = false,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                UpdatedBy = model.UserId,
                CategorieTaskId = model.CategorieTaskId,
                Category = category,
                UserId = model.UserId,
                TenantId = user.TenantId,
                State = TaskState.Todo
            };

            _taskDal.InsertTask(taskToAsign);

            return Ok(taskToAsign);
        }

        public async Task<(bool Success, IActionResult Result)> PatchTaskAsync(int id, TaskPatchRequest request, int userId, int tenantId, byte[]? ifMatchToken)
        {
            var task = await context.Tasks.FirstOrDefaultAsync(t => t.Id == id);
            if (task == null)
                return (false, NotFound(new ProblemDetails { Title = "Task não encontrada", Status = 404 }));

            if (!CanAccessTask(task, userId, tenantId))
                return (false, Forbid());

            var expectedVersion = ifMatchToken ?? ParseRowVersion(request.RowVersion);
            if (expectedVersion == null)
                return (false, BadRequest(new ProblemDetails { Title = "RowVersion é obrigatória", Status = 400 }));

            if (!task.RowVersion.SequenceEqual(expectedVersion))
                return (false, Conflict(BuildConcurrencyProblem(task)));

            if (request.Title != null) task.Title = request.Title;
            if (request.Description != null) task.Description = request.Description;
            if (request.Done.HasValue) task.Done = request.Done.Value;
            if (request.CategorieTaskId.HasValue) task.CategorieTaskId = request.CategorieTaskId.Value;
            if (request.Order.HasValue) task.Order = request.Order.Value;
            if (!string.IsNullOrWhiteSpace(request.State) && Enum.TryParse<TaskState>(request.State, true, out var parsed))
                task.State = parsed;
            if (request.EstimateMinutes.HasValue) task.EstimateMinutes = request.EstimateMinutes;
            if (request.SpentMinutes.HasValue) task.SpentMinutes = request.SpentMinutes;
            if (request.DueDate.HasValue) task.DueDate = request.DueDate;

            task.UpdatedAt = DateTime.UtcNow;
            task.UpdatedBy = userId;

            _logger.LogInformation("Task {TaskId} patch by {UserId} tenant {TenantId}", id, userId, tenantId);

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                await context.Entry(task).ReloadAsync();
                return (false, Conflict(BuildConcurrencyProblem(task)));
            }

            return (true, Ok(ToTaskResponse(task)));
        }

        public async Task<(bool Success, IActionResult Result)> MoveTaskAsync(int id, TaskMoveRequest request, int userId, int tenantId)
        {
            await using var trx = await context.Database.BeginTransactionAsync();

            var task = await context.Tasks.FirstOrDefaultAsync(t => t.Id == id);
            if (task == null)
                return (false, NotFound(new ProblemDetails { Title = "Task não encontrada", Status = 404 }));

            if (!CanAccessTask(task, userId, tenantId))
                return (false, Forbid());

            var expectedVersion = ParseRowVersion(request.RowVersion);
            if (expectedVersion == null)
                return (false, BadRequest(new ProblemDetails { Title = "RowVersion é obrigatória", Status = 400 }));

            if (!task.RowVersion.SequenceEqual(expectedVersion))
                return (false, Conflict(BuildConcurrencyProblem(task)));

            var sourceColumn = task.CategorieTaskId;
            var sourceOrder = task.Order;

            var sourceTasks = await context.Tasks
                .Where(t => t.CategorieTaskId == sourceColumn && t.Id != task.Id)
                .OrderBy(t => t.Order)
                .ToListAsync();

            foreach (var sourceTask in sourceTasks.Where(x => x.Order > sourceOrder))
            {
                sourceTask.Order -= 1;
            }

            var targetTasks = await context.Tasks
                .Where(t => t.CategorieTaskId == request.TargetColumnId && t.Id != task.Id)
                .OrderBy(t => t.Order)
                .ToListAsync();

            var safeOrder = Math.Max(0, Math.Min(request.TargetOrder, targetTasks.Count));

            foreach (var targetTask in targetTasks.Where(x => x.Order >= safeOrder))
            {
                targetTask.Order += 1;
            }

            task.CategorieTaskId = request.TargetColumnId;
            task.Order = safeOrder;
            if (!string.IsNullOrWhiteSpace(request.TargetState) && Enum.TryParse<TaskState>(request.TargetState, true, out var parsedState))
                task.State = parsedState;
            task.Done = task.State == TaskState.Done;
            task.UpdatedAt = DateTime.UtcNow;
            task.UpdatedBy = userId;

            try
            {
                await context.SaveChangesAsync();
                await trx.CommitAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                await context.Entry(task).ReloadAsync();
                return (false, Conflict(BuildConcurrencyProblem(task)));
            }

            _logger.LogInformation("Task {TaskId} move from {SourceColumn} to {TargetColumn} by {UserId} tenant {TenantId}", id, sourceColumn, request.TargetColumnId, userId, tenantId);

            return (true, Ok(ToTaskResponse(task)));
        }

        public async Task<(bool Success, IActionResult Result)> UpdateTaskTimeAsync(int id, TaskTimeRequest request, int userId, int tenantId)
        {
            var task = await context.Tasks.FirstOrDefaultAsync(t => t.Id == id);
            if (task == null)
                return (false, NotFound(new ProblemDetails { Title = "Task não encontrada", Status = 404 }));

            if (!CanAccessTask(task, userId, tenantId))
                return (false, Forbid());

            var expectedVersion = ParseRowVersion(request.RowVersion);
            if (expectedVersion == null)
                return (false, BadRequest(new ProblemDetails { Title = "RowVersion é obrigatória", Status = 400 }));

            if (!task.RowVersion.SequenceEqual(expectedVersion))
                return (false, Conflict(BuildConcurrencyProblem(task)));

            if (request.EstimateMinutes.HasValue) task.EstimateMinutes = request.EstimateMinutes;
            if (request.SpentMinutes.HasValue) task.SpentMinutes = request.SpentMinutes;
            if (request.DueDate.HasValue) task.DueDate = request.DueDate;

            task.UpdatedAt = DateTime.UtcNow;
            task.UpdatedBy = userId;

            await context.SaveChangesAsync();

            return (true, Ok(ToTaskResponse(task)));
        }

        public async Task<IActionResult> GetBoardTasksAsync(int boardId, int userId, int tenantId)
        {
            var tasks = await context.Tasks
                .Where(t => t.CategorieTaskId == boardId && t.TenantId == tenantId && t.UserId == userId)
                .OrderBy(t => t.Order)
                .Select(t => ToTaskResponse(t))
                .ToListAsync();

            return Ok(tasks);
        }

        public static byte[]? ParseRowVersion(string? base64)
        {
            if (string.IsNullOrWhiteSpace(base64)) return null;
            try { return Convert.FromBase64String(base64); }
            catch { return null; }
        }

        private bool CanAccessTask(TaskEntity task, int userId, int tenantId)
            => task.UserId == userId && task.TenantId == tenantId;

        private static ProblemDetails BuildConcurrencyProblem(TaskEntity task)
            => new()
            {
                Title = "Conflito de concorrência",
                Detail = "A tarefa foi alterada por outro processo.",
                Status = 409,
                Extensions = { ["currentRowVersion"] = Convert.ToBase64String(task.RowVersion) }
            };

        private static TaskResponse ToTaskResponse(TaskEntity t) => new()
        {
            Id = t.Id,
            Title = t.Title,
            Description = t.Description,
            Done = t.Done,
            CreatedAt = t.CreatedAt,
            FinishedAt = t.FinishedAt,
            UpdatedAt = t.UpdatedAt,
            UpdatedBy = t.UpdatedBy,
            CategorieTaskId = t.CategorieTaskId,
            Order = t.Order,
            State = t.State.ToString(),
            EstimateMinutes = t.EstimateMinutes,
            SpentMinutes = t.SpentMinutes,
            DueDate = t.DueDate,
            RowVersion = Convert.ToBase64String(t.RowVersion)
        };

        private static System.Linq.Expressions.Expression<Func<TaskEntity, object>> MapLegacyList()
            => task => new
            {
                TaskId = task.Id,
                TaskTitle = task.Title,
                TaskDescription = task.Description,
                Done = task.Done,
                CreatedAt = task.CreatedAt,
                State = task.State,
                Order = task.Order
            };
    }
}
