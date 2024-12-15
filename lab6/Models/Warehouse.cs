using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace lab6.Models;

public partial class Warehouse
{
    public int WarehouseId { get; set; }

    [StringLength(20, ErrorMessage = "Регистрационный номер не должен превышать 20 символов.")]
    [Required(ErrorMessage = "Номер склада обязателено для заполнения.")]
    public string WarehouseNumber { get; set; } = null!;

    [JsonIgnore]
    public virtual ICollection<Fee> Fees { get; set; } = new List<Fee>();

    [JsonIgnore]
    public virtual ICollection<GoodType> GoodTypes { get; set; } = new List<GoodType>();
}
