using CustomMVC.Models;

namespace CustomMVC.ViewModels.FeeViewModel
{
    public class FeeViewModel
    {
        public IEnumerable<Fee> Fees { get; set; }

        public FilterFeeViewModel FilterFeeViewModel { get; set; }

        //Свойство для навигации по страницам
        public PageViewModel PageViewModel { get; set; }
        // Порядок сортировки
        public SortViewModel SortViewModel { get; set; }
    }
}
