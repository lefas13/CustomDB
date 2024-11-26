using CustomMVC.Models;

namespace CustomMVC.ViewModels.GoodTypeViewModel
{
    public class GoodTypeViewModel
    {
        public IEnumerable<GoodType> GoodTypes { get; set; }

        public FilterGoodTypeViewModel FilterGoodTypeViewModel { get; set; }

        //Свойство для навигации по страницам
        public PageViewModel PageViewModel { get; set; }
        // Порядок сортировки
        public SortViewModel SortViewModel { get; set; }
    }
}
