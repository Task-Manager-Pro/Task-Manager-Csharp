namespace Todo.Models
{
    public class TaskMoveRequest
    {
        public int? SourceColumnId { get; set; }
        public int TargetColumnId { get; set; }
        public int TargetOrder { get; set; }
        public string? TargetState { get; set; }
        public string? RowVersion { get; set; }
    }
}
