using System.ComponentModel.DataAnnotations;

namespace ToDoApp.Models
{
    public class Log
    {
        public int Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? Exception { get; set; }
        public string? InnerException { get; set; }
    }
}
