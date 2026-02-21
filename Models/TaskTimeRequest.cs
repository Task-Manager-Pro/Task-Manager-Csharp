namespace Todo.Models
{
    public class TaskTimeRequest
    {
        public int? EstimateMinutes { get; set; }
        public int? SpentMinutes { get; set; }
        public DateTime? DueDate { get; set; }
        public string? RowVersion { get; set; }
    }
}
