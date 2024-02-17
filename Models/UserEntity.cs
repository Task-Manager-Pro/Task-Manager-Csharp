using System.ComponentModel.DataAnnotations.Schema;

namespace Todo.Models
{
    public class UserEntity
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsLogged { get; set; }
        public byte[] ProfilePicture { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public virtual ICollection<TaskEntity> Tasks { get; set; } = new List<TaskEntity>();

    }
}