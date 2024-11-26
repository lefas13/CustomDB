using CustomMVC.Models;

namespace CustomMVC.ViewModels.AgentViewModel
{
    public class AgentViewModel
    {
        public IEnumerable<Agent> Agents { get; set; }

        public FilterAgentViewModel FilterAgentViewModel { get; set; }

        //Свойство для навигации по страницам
        public PageViewModel PageViewModel { get; set; }
        // Порядок сортировки
        public SortViewModel SortViewModel { get; set; }
    }
}
