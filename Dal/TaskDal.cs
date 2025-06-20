using Todo.Data;
using Todo.Domain;

namespace Todo.Dal
{
    public class TaskDal
    {
        private readonly AppDbContext _context;

        public TaskDal(AppDbContext context)
        {
            _context = context;
        }

        public int InsertTask(TaskEntity task)
        {
            _context.Tasks.Add(task);
            _context.SaveChanges();
            return task.Id;
        }

        public TaskEntity UpdateTask(TaskEntity task)
        {
            _context.Tasks.Update(task);
            _context.SaveChanges();
            return task;
        }

        public void DeleteTask(TaskEntity task)
        {
            _context.Tasks.Remove(task);
            _context.SaveChanges();
        }

        public TaskEntity? ToggleDone(int id)
        {
            var task = _context.Tasks.FirstOrDefault(t => t.Id == id);
            if (task == null)
                return null;
            task.Done = !task.Done;
            _context.Tasks.Update(task);
            _context.SaveChanges();
            return task;
        }
    }
}
