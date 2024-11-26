using System.ComponentModel.DataAnnotations;

namespace CustomMVC.ViewModels.AgentViewModel
{
    public class FilterAgentViewModel
    {
        [Display(Name = "Полное имя агента")]
        public string FullName { get; set; } = null!;

        [Display(Name = "Номер агента")]
        public string? IdNumber { get; set; }
    }
}
