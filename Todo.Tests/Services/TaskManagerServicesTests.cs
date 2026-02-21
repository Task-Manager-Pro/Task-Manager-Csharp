using Xunit;
using Microsoft.EntityFrameworkCore;
using Todo.Services;
using Todo.Data;
using Todo.Dal;
using Todo.Models;
using Todo.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;

namespace Todo.Tests.Services
{
    public class TaskManagerServicesTests
    {
        private AppDbContext GetContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new AppDbContext(options);
        }

        [Fact]
        public void InsertTask_Should_Add_Task_And_Return_Ok()
        {
            using var context = GetContext();
            context.CategorieTasks.Add(new CategorieTaskEntity { Id = 1, Name = "Test" });
            context.Users.Add(new UserEntity { Id = 1, Username = "user", Password = "pwd", IsAdmin = false, IsLogged = true, ProfilePicture = Array.Empty<byte>(), TenantId = 1 });
            context.SaveChanges();

            var taskDal = new TaskDal(context);
            var service = new TaskManagerServices(context, taskDal, NullLogger<TaskManagerServices>.Instance);
            var model = new TaskModel { Title = "Sample", Description = "Desc", CategorieTaskId = 1 };

            var result = service.InsertTask(model, 1);

            Assert.IsType<OkObjectResult>(result);
            Assert.Equal(1, context.Tasks.Count());
            var task = context.Tasks.First();
            Assert.Equal("Sample", task.Title);
            Assert.Equal(TaskState.Todo, task.State);
        }

        [Fact]
        public async Task MoveTaskAsync_Should_Reorder_And_Move_Task()
        {
            using var context = GetContext();
            context.Tasks.AddRange(
                new TaskEntity { Id = 1, Title = "T1", CategorieTaskId = 10, UserId = 7, TenantId = 3, Order = 0, RowVersion = Convert.FromBase64String("AQIDBA==") },
                new TaskEntity { Id = 2, Title = "T2", CategorieTaskId = 10, UserId = 7, TenantId = 3, Order = 1, RowVersion = Convert.FromBase64String("AgMEBQ==") },
                new TaskEntity { Id = 3, Title = "T3", CategorieTaskId = 20, UserId = 7, TenantId = 3, Order = 0, RowVersion = Convert.FromBase64String("AwQFBg==") }
            );
            await context.SaveChangesAsync();

            var service = new TaskManagerServices(context, new TaskDal(context), NullLogger<TaskManagerServices>.Instance);
            var req = new TaskMoveRequest
            {
                TargetColumnId = 20,
                TargetOrder = 0,
                TargetState = "InProgress",
                RowVersion = Convert.ToBase64String(context.Tasks.First(t => t.Id == 1).RowVersion)
            };

            var (_, result) = await service.MoveTaskAsync(1, req, 7, 3);
            Assert.IsType<OkObjectResult>(result);

            var moved = context.Tasks.First(t => t.Id == 1);
            var sourceRemaining = context.Tasks.First(t => t.Id == 2);
            var targetExisting = context.Tasks.First(t => t.Id == 3);

            Assert.Equal(20, moved.CategorieTaskId);
            Assert.Equal(0, moved.Order);
            Assert.Equal(0, sourceRemaining.Order);
            Assert.Equal(1, targetExisting.Order);
        }

        [Fact]
        public async Task PatchTaskAsync_Should_Return_Conflict_When_RowVersion_Differs()
        {
            using var context = GetContext();
            context.Tasks.Add(new TaskEntity
            {
                Id = 99,
                Title = "Concurrency",
                CategorieTaskId = 1,
                UserId = 2,
                TenantId = 2,
                RowVersion = Convert.FromBase64String("AQID")
            });
            await context.SaveChangesAsync();

            var service = new TaskManagerServices(context, new TaskDal(context), NullLogger<TaskManagerServices>.Instance);
            var patch = new TaskPatchRequest { Title = "Changed", RowVersion = Convert.ToBase64String(Convert.FromBase64String("BAUG")) };

            var (_, result) = await service.PatchTaskAsync(99, patch, 2, 2, null);
            Assert.IsType<ConflictObjectResult>(result);
        }
    }
}
