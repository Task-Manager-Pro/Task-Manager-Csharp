using Todo.Data;
using Todo.Domain;

namespace Todo.Dal
{
    public class UserDal
    {
        private readonly AppDbContext _context;

        public UserDal(AppDbContext context)
        {
            _context = context;
        }

        public void AddUser(UserEntity user)
        {
            _context.Users.Add(user);
            _context.SaveChanges();
        }
    }
}
