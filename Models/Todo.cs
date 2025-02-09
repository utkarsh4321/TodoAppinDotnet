namespace todoApis.Models
{
    public class Todo
    {
        public int Id { get; set; }
        public string TodoName { get; set; }
        public bool Status { get; set; }
        public int UserId { get; set; }

        public User? User { get; set; } = null!;
    }
}
