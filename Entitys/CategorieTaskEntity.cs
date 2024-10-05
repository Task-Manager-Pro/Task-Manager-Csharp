using System.ComponentModel.DataAnnotations.Schema;

namespace Todo.Domain
{
    public class CategorieTaskEntity
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }

        public virtual ICollection<TaskEntity>? Tasks { get; set; }
    }
}
