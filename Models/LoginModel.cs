namespace Todo.Models
{
    public class LoginModel
    {
        //hasnokey
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool IsAdmin { get; set; }
        public bool isLogged { get; set; }
    }
}