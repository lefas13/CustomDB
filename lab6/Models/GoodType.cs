using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace lab6.Models;

public partial class GoodType
{
    public int GoodTypeId { get; set; }

    public string Name { get; set; } = null!;

    public string Measurement { get; set; } = null!;

    public decimal AmountOfFee { get; set; }

    [JsonIgnore]
    public virtual ICollection<Good> Goods { get; set; } = new List<Good>();

    [JsonIgnore]
    public virtual ICollection<Warehouse> Warehouses { get; set; } = new List<Warehouse>();
}
