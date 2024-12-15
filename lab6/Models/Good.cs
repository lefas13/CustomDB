using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace lab6.Models;

public partial class Good
{
    public int GoodId { get; set; }

    public string Name { get; set; } = null!;

    public int GoodTypeId { get; set; }

    [JsonIgnore]
    public virtual ICollection<Fee> Fees { get; set; } = new List<Fee>();

    [ForeignKey("GoodTypeId")]
    public virtual GoodType GoodType { get; set; } = null!;
}
