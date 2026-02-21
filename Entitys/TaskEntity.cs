using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Todo.Domain;

public class TaskEntity
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public bool Done { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? FinishedAt { get; set; }
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public int UpdatedBy { get; set; }
    public int CategorieTaskId { get; set; }
    public int Order { get; set; }
    public TaskState State { get; set; } = TaskState.Todo;
    public int? EstimateMinutes { get; set; }
    public int? SpentMinutes { get; set; }
    public DateTime? DueDate { get; set; }
    public int TenantId { get; set; } = 1;

    [Timestamp]
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();

    [ForeignKey("CategorieTaskId")]
    public virtual CategorieTaskEntity? Category { get; set; }

    [ForeignKey("UserId")]
    public virtual UserEntity? User { get; set; }

    public int UserId { get; set; }
}
