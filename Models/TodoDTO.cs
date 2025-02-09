using System.Globalization;

namespace todoApis.Models
{
    public class TodoDTO
    {
        public int Id { get; set; }
        public string TodoName { get; set; }

        public bool Status { get; set; }

        public int userId {  get; set; }

    }
}
