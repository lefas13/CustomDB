using System;
using System.Collections.Generic;

namespace CustomApp.Models;

public partial class Warehouse
{
    public int WarehouseId { get; set; }

    public string WarehouseNumber { get; set; } = null!;

    public virtual ICollection<Fee> Fees { get; set; } = new List<Fee>();

    public virtual ICollection<GoodType> GoodTypes { get; set; } = new List<GoodType>();
}
