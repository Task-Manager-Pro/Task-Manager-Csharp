namespace Todo.Models
{
     public class TaskEntity
     {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Description {get; set;}
        public bool Done { get; set; }
        public DateTime CreatedAt { get; set; }
     }
}   