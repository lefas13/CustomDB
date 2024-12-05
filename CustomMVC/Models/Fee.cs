using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CustomMVC.Models;

public partial class Fee
{
    public int FeeId { get; set; }

    public int WarehouseId { get; set; }

    public int GoodId { get; set; }

    public DateOnly ReceiptDate { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Количество не может быть меньше 0.")]
    public int Amount { get; set; }

    public string DocumentNumber { get; set; } = null!;

    public int AgentId { get; set; }

    [Range(1, double.MaxValue, ErrorMessage = "Сумма не может быть меньше 0.")]
    public decimal FeeAmount { get; set; }

    public DateOnly? PaymentDate { get; set; }

    public DateOnly? ExportDate { get; set; }

    public virtual Agent Agent { get; set; } = null!;

    public virtual Good Good { get; set; } = null!;

    public virtual Warehouse Warehouse { get; set; } = null!;
}
