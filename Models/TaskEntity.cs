using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace Todo.Models;

 public class TaskEntity
 {
    public int Id { get; set; }
    public string? Title { get; set; }
    public string? Description {get; set;}
    public bool Done { get; set; }
    public DateTime CreatedAt { get; set; }
    public int CategorieTaskId { get; set; } 
    public virtual CategorieTaskEntity CategorieTask { get; set; }
}
