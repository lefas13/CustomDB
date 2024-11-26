using System.ComponentModel.DataAnnotations;

namespace CustomMVC.ViewModels.WarehouseViewModel
{
    public class FilterWarehouseViewModel
    {
        [Display(Name = "Номер склада")]
        public string? WarehouseNumber { get; set; }
    }
}
