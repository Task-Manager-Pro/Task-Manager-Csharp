using Xunit;
using Microsoft.EntityFrameworkCore;
using Todo.Services;
using Todo.Data;
using Todo.Dal;
using Todo.Models;
using Todo.Domain;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace Todo.Tests.Services
{
    public class TaskManagerServicesTests
    {
        private AppDbContext GetContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: System.Guid.NewGuid().ToString())
                .Options;
            return new AppDbContext(options);
        }

        [Fact]
        public void InsertTask_Should_Add_Task_And_Return_Ok()
        {
            using var context = GetContext();
            context.CategorieTasks.Add(new CategorieTaskEntity { Id = 1, Name = "Test" });
            context.SaveChanges();

            var taskDal = new TaskDal(context);
            var service = new TaskManagerServices(context, taskDal);
            var model = new TaskModel { Title = "Sample", Description = "Desc", CategorieTaskId = 1 };

            var result = service.InsertTask(model, 1);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(1, context.Tasks.Count());
            var task = context.Tasks.First();
            Assert.Equal("Sample", task.Title);
        }
    }
}
