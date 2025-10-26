using Todo.Domain;
using Todo.Services;

namespace Todo.Interfaces
{

        public interface ILoginService
        {
        Task<AuthResult> AuthenticateAsync(string username, string password, CancellationToken ct = default);
        Task<bool> CreateAccountAsync(UserEntity model, CancellationToken ct = default);
        Task<List<UserEntity>> ListUsersAsync(CancellationToken ct = default);
        }
}