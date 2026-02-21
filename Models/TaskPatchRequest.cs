namespace Todo.Models
{
    public class TaskPatchRequest
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public bool? Done { get; set; }
        public int? CategorieTaskId { get; set; }
        public int? Order { get; set; }
        public string? State { get; set; }
        public int? EstimateMinutes { get; set; }
        public int? SpentMinutes { get; set; }
        public DateTime? DueDate { get; set; }
        public string? RowVersion { get; set; }
    }
}
