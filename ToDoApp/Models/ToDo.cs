using System.ComponentModel.DataAnnotations;

namespace ToDoApp.Models
{
    public class ToDo
    {
        public int Id { get; set; }
        [Required]
        public string? Description { get; set; }
        public bool Check { get; set; }

    }
}
