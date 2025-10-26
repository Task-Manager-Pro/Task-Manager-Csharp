using Todo.Domain;

namespace Todo.Interfaces
{
    public interface IJwtTokenService
    {
        string Generate(UserEntity user);
    }
}
