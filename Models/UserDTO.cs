namespace todoApis.Models
{
    public class UserDTO
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }

        public List<TodoDTO> Todos { get; set; } = new List<TodoDTO>();
    }
}
