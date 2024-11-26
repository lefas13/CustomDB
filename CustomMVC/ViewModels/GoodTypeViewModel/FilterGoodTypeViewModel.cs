using System.ComponentModel.DataAnnotations;

namespace CustomMVC.ViewModels.GoodTypeViewModel
{
    public class FilterGoodTypeViewModel
    {
        [Display(Name = "Название типа")]
        public string Type { get; set; } = null!;

        [Display(Name = "Единица измерения")]
        public string? Measurement { get; set; }

        [Display(Name = "Стоимость пошлины")]
        public decimal? AmountOfFee { get; set; }
    }
}
