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
                var tasksDone = _taskManagerServices.GetTaskDone();
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
                var allTasks = _taskManagerServices.GetAllTasks();
                return Ok(allTasks);
            }catch(System.Exception) 
            { 
                return BadRequest();
            }
        }

        [Authorize]
        [HttpGet("ListTaskByUser/{userId}")]
        public IActionResult ListTarefaByUser(
        [FromRoute] int userId)
        {
            try
            {
                var tasksByUser = _taskManagerServices.GetTasksByUser(userId);
                return Ok(tasksByUser);
            }catch(System.Exception)
            {
                return BadRequest();
            }
        }

        [HttpGet("/GetById/{id:int}")]
        public IActionResult GetById([FromRoute] int id)
        {
           try
           {
                var taskById = _taskManagerServices.GetById(id);
                return Ok(taskById);
           }catch(System.Exception)
            {
               return BadRequest();
           }
        }

        [HttpPost("/insertTask/{userId}")]
        public IActionResult Post(
        [FromBody] TaskEntity model,
        [FromRoute] int userId)
        {
            try
            {
                var task = _taskManagerServices.InsertTask(model, userId);
                return Ok(task);
            }catch(System.Exception)
            {
                return BadRequest();
            }   
        }
           
        [HttpPut("/edit/{id:int}")]
        public IActionResult Put(
        [FromRoute] int id,
        [FromBody] TaskEntity model)
        {
           try
           {
                var taskToEdit = _taskManagerServices.EditTask(model, id);
                return Ok(taskToEdit);
           }catch(System.Exception)
           {
                return BadRequest();
           }
        }

        [HttpDelete("/delete/{id:int}")]
        public IActionResult Delete(
        [FromRoute] int id)
        {
            try
            {
                var editeTask = _taskManagerServices.DeleteTask(id);
                return Ok(editeTask);
            }catch(System.Exception)
            {
                return BadRequest();
            }
        }

        [HttpPut("/done/{id:int}")]
        public IActionResult Done(
        [FromRoute] int id)
        {
            try
            {
                var taskDone = _taskManagerServices.DoneTask(id);
                return Ok(taskDone);
            }catch(System.Exception)
            {
                return BadRequest();
            }
        }

        [HttpPost("/asignTask")]
        public IActionResult AsignTask(
            [FromBody] TaskEntity model)
        {
            try
            {
                var asignTask = _taskManagerServices.AsignTask(model);
                return Ok(asignTask);
            }catch(System.Exception)
            {
                return BadRequest();
            }
        }

    }
}