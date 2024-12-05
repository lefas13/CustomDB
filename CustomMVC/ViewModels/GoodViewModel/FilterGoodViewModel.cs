using System.ComponentModel.DataAnnotations;

namespace CustomMVC.ViewModels.GoodViewModel
{
    public class FilterGoodViewModel
    {
        [Display(Name = "Название типа")]
        public string? Type { get; set; }

        [Display(Name = "Название товара")]
        public string? Good { get; set; }
    }
}
