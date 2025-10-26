using Microsoft.EntityFrameworkCore;
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

        /// <summary>
        /// Adiciona um novo usuário ao banco de dados.
        /// </summary>
        /// <param name="model">Entidade do usuário a ser criada.</param>
        /// <param name="ct">Token de cancelamento (opcional).</param>
        public async Task AddUserAsync(UserEntity model, CancellationToken ct = default)
        {
            if (model is null)
                throw new ArgumentNullException(nameof(model), "O modelo de usuário não pode ser nulo.");

            // Evita duplicidade de Username
            var exists = await _context.Users
                .AsNoTracking()
                .AnyAsync(u => u.Username == model.Username, ct);

            if (exists)
                throw new InvalidOperationException($"O nome de usuário '{model.Username}' já está em uso.");

            await _context.Users.AddAsync(model, ct);
            await _context.SaveChangesAsync(ct);
        }
    }
}
