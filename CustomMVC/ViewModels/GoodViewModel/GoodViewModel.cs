using CustomMVC.Models;

namespace CustomMVC.ViewModels.GoodViewModel
{
    public class GoodViewModel
    {
        public IEnumerable<Good> Goods { get; set; }

        public FilterGoodViewModel FilterGoodViewModel { get; set; }

        //Свойство для навигации по страницам
        public PageViewModel PageViewModel { get; set; }
        // Порядок сортировки
        public SortViewModel SortViewModel { get; set; }
    }
}
