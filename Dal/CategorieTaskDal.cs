using Todo.Data;
using Todo.Domain;

namespace Todo.Dal
{
    public class CategorieTaskDal
    {
        private readonly AppDbContext _context;

        public CategorieTaskDal(AppDbContext context)
        {
            _context = context;
        }

        public void AddCategorieTask(CategorieTaskEntity entity)
        {
            _context.CategorieTasks.Add(entity);
            _context.SaveChanges();
        }

        public void UpdateCategorieTask(CategorieTaskEntity entity)
        {
            _context.CategorieTasks.Update(entity);
            _context.SaveChanges();
        }
    }
}
