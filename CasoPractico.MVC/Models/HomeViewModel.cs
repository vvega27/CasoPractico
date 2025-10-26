using CasoPractico.Model.DTOs;

namespace MVC.Models
{
    public class HomeViewModel
    {
        public string Title { get; set; } = "Task";
        public IEnumerable<TaskDTO> Tasks { get; set; } = [];
    }
}
