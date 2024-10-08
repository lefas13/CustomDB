using System;
using System.Collections.Generic;

namespace CustomApp.Models;

public partial class Fee
{
    public int FeeId { get; set; }

    public int WarehouseId { get; set; }

    public int GoodId { get; set; }

    public DateOnly ReceiptDate { get; set; }

    public int Amount { get; set; }

    public string DocumentNumber { get; set; } = null!;

    public int AgentId { get; set; }

    public decimal FeeAmount { get; set; }

    public DateOnly? PaymentDate { get; set; }

    public DateOnly? ExportDate { get; set; }

    public virtual Agent Agent { get; set; } = null!;

    public virtual Good Good { get; set; } = null!;

    public virtual Warehouse Warehouse { get; set; } = null!;
}
