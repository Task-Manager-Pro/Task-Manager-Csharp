using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Todo.Models;
using Todo.Services;

namespace Todo.Controllers
{
    [ApiController]
    public class TaskManagerController : ControllerBase
    {
        private readonly TaskManagerServices _taskManagerServices;

        public TaskManagerController(TaskManagerServices taskManagerServices)
        {
            _taskManagerServices = taskManagerServices;
        }

        [Authorize]
        [HttpGet("/TasksToDo")]
        public IActionResult Get()
        {
            try
            {
                var tasksToDo = _taskManagerServices.GetTasksToDo();
                return Ok(tasksToDo);
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpGet("/ListTaskDone")]
        public IActionResult ListTaskDone()
        {
            try
            {
                var tasksDone = _taskManagerServices.GetTaskDone();
                return Ok(tasksDone);
            }
            catch
            {
                return BadRequest();
            }
        }

        [Authorize]
        [HttpGet("/ListllTasks")]
        public IActionResult ListAllTasks()
        {
            try
            {
                var allTasks = _taskManagerServices.GetAllTasks();
                return Ok(allTasks);
            }
            catch
            {
                return BadRequest();
            }
        }

        [Authorize]
        [HttpGet("ListTaskByUser/{userId}")]
        public IActionResult ListTarefaByUser([FromRoute] int userId)
        {
            try
            {
                var tasksByUser = _taskManagerServices.GetTasksByUser(userId);
                return Ok(tasksByUser);
            }
            catch
            {
                return BadRequest();
            }
        }

        [Authorize]
        [HttpGet("/GetById/{id:int}")]
        public IActionResult GetById([FromRoute] int id)
        {
            try
            {
                var taskById = _taskManagerServices.GetById(id);
                return Ok(taskById);
            }
            catch
            {
                return BadRequest();
            }
        }

        [Authorize]
        [HttpPost("/insertTask/{userId}")]
        public IActionResult Post([FromBody] TaskModel model, [FromRoute] int userId)
        {
            try
            {
                var task = _taskManagerServices.InsertTask(model, userId);
                return Ok(task);
            }
            catch
            {
                return BadRequest();
            }
        }

        [Authorize]
        [HttpPut("/edit/{id:int}")]
        public IActionResult Put([FromRoute] int id, [FromBody] TaskModel model)
        {
            try
            {
                var taskToEdit = _taskManagerServices.EditTask(model, id);
                return Ok(taskToEdit);
            }
            catch
            {
                return BadRequest();
            }
        }

        [Authorize]
        [HttpDelete("/delete/{id:int}")]
        public IActionResult Delete([FromRoute] int id)
        {
            try
            {
                var editeTask = _taskManagerServices.DeleteTask(id);
                return Ok(editeTask);
            }
            catch
            {
                return BadRequest();
            }
        }

        [Authorize]
        [HttpPut("/done/{id:int}")]
        public IActionResult Done([FromRoute] int id)
        {
            try
            {
                var taskDone = _taskManagerServices.DoneTask(id);
                return Ok(taskDone);
            }
            catch
            {
                return BadRequest();
            }
        }

        [Authorize]
        [HttpPost("/asignTask")]
        public IActionResult AsignTask([FromBody] TaskModel model)
        {
            try
            {
                var asignTask = _taskManagerServices.AsignTask(model);
                return Ok(asignTask);
            }
            catch
            {
                return BadRequest();
            }
        }

        [Authorize]
        [HttpPatch("/api/tasks/{id:int}")]
        public async Task<IActionResult> PatchTask([FromRoute] int id, [FromBody] TaskPatchRequest request)
        {
            var (userId, tenantId) = ResolveIdentity();
            if (userId == null || tenantId == null)
                return Unauthorized(new ProblemDetails { Title = "Token inválido", Status = 401 });

            var ifMatchHeader = Request.Headers.IfMatch.FirstOrDefault()?.Trim('"');
            var ifMatch = TaskManagerServices.ParseRowVersion(ifMatchHeader);

            var (_, result) = await _taskManagerServices.PatchTaskAsync(id, request, userId.Value, tenantId.Value, ifMatch);
            return result;
        }

        [Authorize]
        [HttpPost("/api/tasks/{id:int}/move")]
        public async Task<IActionResult> MoveTask([FromRoute] int id, [FromBody] TaskMoveRequest request)
        {
            var (userId, tenantId) = ResolveIdentity();
            if (userId == null || tenantId == null)
                return Unauthorized(new ProblemDetails { Title = "Token inválido", Status = 401 });

            var (_, result) = await _taskManagerServices.MoveTaskAsync(id, request, userId.Value, tenantId.Value);
            return result;
        }

        [Authorize]
        [HttpPut("/api/tasks/{id:int}/time")]
        public async Task<IActionResult> UpdateTime([FromRoute] int id, [FromBody] TaskTimeRequest request)
        {
            var (userId, tenantId) = ResolveIdentity();
            if (userId == null || tenantId == null)
                return Unauthorized(new ProblemDetails { Title = "Token inválido", Status = 401 });

            var (_, result) = await _taskManagerServices.UpdateTaskTimeAsync(id, request, userId.Value, tenantId.Value);
            return result;
        }

        [Authorize]
        [HttpGet("/api/boards/{boardId:int}/tasks")]
        public async Task<IActionResult> GetBoardTasks([FromRoute] int boardId)
        {
            var (userId, tenantId) = ResolveIdentity();
            if (userId == null || tenantId == null)
                return Unauthorized(new ProblemDetails { Title = "Token inválido", Status = 401 });

            return await _taskManagerServices.GetBoardTasksAsync(boardId, userId.Value, tenantId.Value);
        }

        private (int? userId, int? tenantId) ResolveIdentity()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var tenantClaim = User.FindFirstValue("tenant_id") ?? User.FindFirstValue("tenantId");

            if (!int.TryParse(userIdClaim, out var userId)) return (null, null);
            if (!int.TryParse(tenantClaim, out var tenantId)) tenantId = 1;

            return (userId, tenantId);
        }
    }
}
