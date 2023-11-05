namespace Todo.Models
{
    public class LoginModel
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool IsAdmin { get; set; }
        public bool isLogged { get; set; }
        public DateTime LoginTime { get; set; } = DateTime.Now;
    }
}