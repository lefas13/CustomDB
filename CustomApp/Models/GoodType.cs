using System;
using System.Collections.Generic;

namespace CustomApp.Models;

public partial class GoodType
{
    public int GoodTypeId { get; set; }

    public string Name { get; set; } = null!;

    public string Measurement { get; set; } = null!;

    public decimal AmountOfFee { get; set; }

    public virtual ICollection<Good> Goods { get; set; } = new List<Good>();

    public virtual ICollection<Warehouse> Warehouses { get; set; } = new List<Warehouse>();
}
