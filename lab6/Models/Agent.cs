using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace lab6.Models;

public partial class Agent
{
    public int AgentId { get; set; }

    [StringLength(255, ErrorMessage = "Инициалы не должны превышать 225 символов.")]
    [Required(ErrorMessage = "Имя агента обязателено для заполнения.")]
    public string FullName { get; set; } = null!;

    [Required(ErrorMessage = "ID агента обязателен для заполнения.")]
    [StringLength(20, ErrorMessage = "Регистрационный номер не должен превышать 20 символов.")]
    public string IdNumber { get; set; } = null!;

    [JsonIgnore]
    public virtual ICollection<Fee> Fees { get; set; } = new List<Fee>();
}
