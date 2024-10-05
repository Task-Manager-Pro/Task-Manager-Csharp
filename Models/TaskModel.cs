namespace Todo.Models
{
    public class TaskModel
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public bool Done { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime FinishedAt { get; set; }
        public int CategorieTaskId { get; set; }
        public int UserId { get; set; }

        public string? Category { get; set; }
        public string? UserName { get; set; }
    }
}
