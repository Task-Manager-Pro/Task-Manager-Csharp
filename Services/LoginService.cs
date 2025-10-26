using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using Todo.Dal;
using Todo.Data;
using Todo.Domain;
using Todo.Interfaces;
using Todo.Models;

namespace Todo.Services
{
    public sealed class LoginService : ILoginService
    {
        private readonly AppDbContext _context;
        private readonly UserDal _dal;
        private readonly IJwtTokenService _jwt;

        public LoginService(AppDbContext context, UserDal dal, IJwtTokenService jwt)
        {
            _context = context;
            _dal = dal;
            _jwt = jwt;
        }

        public async Task<List<UserEntity>> ListUsersAsync(CancellationToken ct = default)
        {
            return await _context.Users.AsNoTracking().ToListAsync(ct);
        }


        public async Task<bool> CreateAccountAsync(UserEntity model, CancellationToken ct = default)
        {
            if (model is null || string.IsNullOrWhiteSpace(model.Username) || string.IsNullOrWhiteSpace(model.Password))
                return false;

            model.Password = ComputeSha256HexUpper(model.Password);
            model.CreatedAt = model.CreatedAt == default ? DateTime.UtcNow : model.CreatedAt;

            await _dal.AddUserAsync(model, ct);
            return true;
        }

        public async Task<AuthResult> AuthenticateAsync(string username, string password, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                return AuthResult.Fail();

            var user = await _context.Users.AsNoTracking()
                .SingleOrDefaultAsync(u => u.Username == username, ct);

            if (user is null)
                return AuthResult.Fail();

            var incomingHash = ComputeSha256HexUpper(password);

            if (!SecureEquals(incomingHash, user.Password))
                return AuthResult.Fail();

            var token = _jwt.Generate(user);

            return AuthResult.Success(token, new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                IsAdmin = user.IsAdmin,
                IsLogged = true
            });
        }



        private static string ComputeSha256HexUpper(string input)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
            var sb = new StringBuilder(bytes.Length * 2);
            foreach (var b in bytes) sb.Append(b.ToString("X2")); // maiúsculo (mesmo padrão do CONVERT(...,2))
            return sb.ToString();
        }

        private static bool SecureEquals(string a, string b)
        {
            if (a is null || b is null) return false;
            var ba = Encoding.UTF8.GetBytes(a);
            var bb = Encoding.UTF8.GetBytes(b);
            return CryptographicOperations.FixedTimeEquals(ba, bb);
        }
    }

    public sealed class AuthResult
    {
        public bool Succeeded { get; private set; }
        public string? Token { get; private set; }
        public UserDto? User { get; private set; }

        public static AuthResult Success(string token, UserDto user) => new() { Succeeded = true, Token = token, User = user };
        public static AuthResult Fail() => new() { Succeeded = false };
    }

    public sealed class UserDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = "";
        public bool IsAdmin { get; set; }
        public bool IsLogged { get; set; }
    }
}
