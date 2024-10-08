using System;
using System.Collections.Generic;

namespace CustomApp.Models;

public partial class Agent
{
    public int AgentId { get; set; }

    public string FullName { get; set; } = null!;

    public string IdNumber { get; set; } = null!;

    public virtual ICollection<Fee> Fees { get; set; } = new List<Fee>();
}
