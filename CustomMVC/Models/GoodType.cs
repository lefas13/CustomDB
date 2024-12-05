using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CustomMVC.Models;

public partial class GoodType
{
    public int GoodTypeId { get; set; }

    public string Name { get; set; } = null!;

    public string Measurement { get; set; } = null!;

    [Range(1, double.MaxValue, ErrorMessage = "Сумма не может быть меньше 0.")]
    public decimal AmountOfFee { get; set; }

    public virtual ICollection<Good> Goods { get; set; } = new List<Good>();

    public virtual ICollection<Warehouse> Warehouses { get; set; } = new List<Warehouse>();
}
