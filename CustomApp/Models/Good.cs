using System;
using System.Collections.Generic;

namespace CustomApp.Models;

public partial class Good
{
    public int GoodId { get; set; }

    public string Name { get; set; } = null!;

    public int GoodTypeId { get; set; }

    public virtual ICollection<Fee> Fees { get; set; } = new List<Fee>();

    public virtual GoodType GoodType { get; set; } = null!;
}
