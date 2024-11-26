using CustomMVC.Models;
using System.ComponentModel.DataAnnotations;

namespace CustomMVC.ViewModels.FeeViewModel
{
    public class FilterFeeViewModel
    {
        [Display(Name = "Полное имя агента")]
        public string? FullName { get; set; } 

        [Display(Name = "Номер склада")]
        public string? WarehouseNumber { get; set; }

        [Display(Name = "Название товара")]
        public string? Good { get; set; }

        [Display(Name = "Номер документа")]
        public string? DocumentNumber { get; set; }

        [Display(Name = "Количество товаров")]
        public int? Amount { get; set; }

        [Display(Name = "Стоимость пошлины")]
        public decimal? FeeAmount { get; set; }

        [Display(Name = "Дата получения")]
        public DateOnly? ReceiptDate { get; set; }

        [Display(Name = "Дата оплаты")]
        public DateOnly? PaymentDate { get; set; }

        [Display(Name = "Дата отправки")]
        public DateOnly? ExportDate { get; set; }

    }
}
