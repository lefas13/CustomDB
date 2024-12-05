using CustomMVC.Models;

namespace CustomMVC.ViewModels.CustomInformationViewModel
{
    public class CustomInformationViewModel
    {
        public IEnumerable<Warehouse> Warehouses { get; set; }
        public IEnumerable<Agent> Agents { get; set; } // Список агентов
        public int SelectedAgentId { get; set; } // ID выбранного агента
        public int SelectedWarehouseId { get; set; } // ID выбранного склада

        public Dictionary<int, List<GoodDetails>> GoodsDetails { get; set; } // Сведения по номенклатуре и количеству товаров на складе
        public Dictionary<string, int> TotalGoodsCount { get; set; } // Суммарное количество каждого вида товара на всех складах
        public Dictionary<string, int> GoodsCountOnWarehouse { get; set; } // Количество товара на заданном складе за период
        public Dictionary<string, double> AverageStayTime { get; set; } // Среднее время нахождения товара на складе

        public decimal TotalFeesForPeriod { get; set; } // Суммарные пошлины за период
        public decimal FeesByWarehouseAndAgent { get; set; } // Сумма пошлин по складу и агенту за текущий месяц
        public decimal TotalFeesForYear { get; set; } // Итоговая сумма всех пошлин за текущий год

        public PagingInfo PagingInfo { get; set; }
    }
}

