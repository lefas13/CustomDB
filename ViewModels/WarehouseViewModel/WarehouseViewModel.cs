using CustomMVC.Models;

namespace CustomMVC.ViewModels.WarehouseViewModel
{
    public class WarehouseViewModel
    {
        public IEnumerable<Warehouse> Warehouses { get; set; }

        public FilterWarehouseViewModel FilterWarehouseViewModel { get; set; }

        //Свойство для навигации по страницам
        public PageViewModel PageViewModel { get; set; }
        // Порядок сортировки
        public SortViewModel SortViewModel { get; set; }
    }
}
