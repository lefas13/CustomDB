namespace CustomMVC.ViewModels
{
    public enum SortState
    {
        No,                         // не сортировать
        FullNameAsc,                // по имени агента по возрастанию
        FullNameDesc,               // по имени агента по убыванию
        IdNumberAsc,                // по номеру агента по возрастанию
        IdNumberDesc,               // по номеру агента по убыванию
        WarehouseNumberAsc,         // по номеру склада по возрастанию
        WarehouseNumberDesc,        // по номеру склада по убыванию
        TypeAsc,                    // по типу товара по возрастанию
        TypeDesc,                   // по типу товара по убыванию
        MeasurementAsc,             // по единице измерения по возрастанию
        MeasurementDesc,            // по единице измерения по убыванию
        GoodAsc,                    // по товару по возрастанию
        GoodDesc,                   // по товару по убыванию
        DocumentNumberAsc,          // по номеру документа по возрастанию
        DocumentNumberDesc,         // по номеру документа по убыванию
        ReceiptDateAsc,             // по дате получения по возрастанию
        ReceiptDateDesc,            // по дате получения по убыванию
        PaymentDateAsc,             // по дате оплаты по возрастанию
        PaymentDateDesc,            // по дате оплаты по убыванию
        ExportDateAsc,              // по дате отправки по возрастанию
        ExportDateDesc,             // по дате отправки по убыванию
    }

    public class SortViewModel
    {
        public SortState FullNameSort { get; set; }             // Сортировка по имени агента
        public SortState IdNumberSort { get; set; }             // Сортировка по номеру агента
        public SortState WarehouseNumberSort { get; set; }      // Сортировка по номеру склада
        public SortState TypeSort { get; set; }                 // Сортировка по типу товара
        public SortState MeasurementSort { get; set; }          // Сортировка по единице измериния
        public SortState GoodSort { get; set; }                 // Сортировка по товару
        public SortState DocumentNumberSort { get; set; }       // Сортировка по номеру документа
        public SortState ReceiptDateSort { get; set; }          // Сортировка по дате получения
        public SortState PaymentDateSort { get; set; }          // Сортировка по дате оплаты
        public SortState ExportDateSort { get; set; }           // Сортировка по дате отправки
        public SortState CurrentState { get; set; }             // Текущее состояние сортировки

        public SortViewModel(SortState sortOrder)
        {
            // Установка сортировки для имени агента
            FullNameSort = sortOrder == SortState.FullNameAsc ? SortState.FullNameDesc : SortState.FullNameAsc;

            // Установка сортировки для номера агента
            IdNumberSort = sortOrder == SortState.IdNumberAsc ? SortState.IdNumberDesc : SortState.IdNumberAsc;

            // Установка сортировки для номера склада
            WarehouseNumberSort = sortOrder == SortState.WarehouseNumberAsc ? SortState.WarehouseNumberDesc : SortState.WarehouseNumberAsc;

            // Установка сортировки для типа товара
            TypeSort = sortOrder == SortState.TypeAsc ? SortState.TypeDesc : SortState.TypeAsc;

            // Установка сортировки для единицы измериния
            MeasurementSort = sortOrder == SortState.MeasurementAsc ? SortState.MeasurementDesc : SortState.MeasurementAsc;

            // Установка сортировки для товара
            GoodSort = sortOrder == SortState.GoodAsc ? SortState.GoodDesc : SortState.GoodAsc;

            // Установка сортировки для номера документа
            DocumentNumberSort = sortOrder == SortState.DocumentNumberAsc ? SortState.DocumentNumberDesc : SortState.DocumentNumberAsc;

            // Установка сортировки для даты получения
            ReceiptDateSort = sortOrder == SortState.ReceiptDateAsc ? SortState.ReceiptDateDesc : SortState.ReceiptDateAsc;

            // Установка сортировки для даты оплаты
            PaymentDateSort = sortOrder == SortState.PaymentDateAsc ? SortState.PaymentDateDesc : SortState.PaymentDateAsc;

            // Установка сортировки для даты отправки
            ExportDateSort = sortOrder == SortState.ExportDateAsc ? SortState.ExportDateDesc : SortState.ExportDateAsc;

            // Установка текущего состояния сортировки
            CurrentState = sortOrder;
        }
    }
}
