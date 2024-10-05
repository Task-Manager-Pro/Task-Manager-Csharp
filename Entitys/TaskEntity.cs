using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace Todo.Domain;

 public class TaskEntity
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public bool Done { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime FinishedAt { get; set; }
    public int CategorieTaskId { get; set; }

    [ForeignKey("CategorieTaskId")]
    public virtual CategorieTaskEntity? Category { get; set; }

    [ForeignKey("UserId")]
    public virtual UserEntity? User { get; set; }

    public int UserId { get; set; }

}
