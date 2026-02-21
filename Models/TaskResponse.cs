namespace Todo.Models
{
    public class TaskResponse
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public bool Done { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? FinishedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int UpdatedBy { get; set; }
        public int CategorieTaskId { get; set; }
        public int Order { get; set; }
        public string State { get; set; } = "Todo";
        public int? EstimateMinutes { get; set; }
        public int? SpentMinutes { get; set; }
        public DateTime? DueDate { get; set; }
        public string RowVersion { get; set; } = string.Empty;
    }
}
