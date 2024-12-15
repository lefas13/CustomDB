using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace lab6.Models;

public partial class Fee
{
    public int FeeId { get; set; }

    [Required(ErrorMessage = "Склад обязателен для заполнения.")]
    public int WarehouseId { get; set; }

    [Required(ErrorMessage = "Товар обязателен для заполнения.")]
    public int GoodId { get; set; }

    [Required(ErrorMessage = "Дата получения обязательна для заполнения.")]
    public DateOnly? ReceiptDate { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Количество должно быть положительным числом.")]
    [Required(ErrorMessage = "Количество товаров обязательно для заполнения.")]
    public int Amount { get; set; }

    [StringLength(20, ErrorMessage = "Регистрационный номер не должен превышать 20 символов.")]
    [Required(ErrorMessage = "Номер документа обязателен для заполнения.")]
    public string DocumentNumber { get; set; } = null!;

    [Required(ErrorMessage = "Агент обязателен для заполнения.")]
    public int AgentId { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Цена должна быть положительным числом.")]
    [Required(ErrorMessage = "Стоимость пошлины обязательна для заполнения.")]
    public decimal FeeAmount { get; set; }

    [Required(ErrorMessage = "Дата оплаты обязательна для заполнения.")]
    public DateOnly? PaymentDate { get; set; }

    [Required(ErrorMessage = "Дата отправки обязательна для заполнения.")]
    public DateOnly? ExportDate { get; set; }

    [ForeignKey("AgentId")]
    public virtual Agent Agent { get; set; } = null!;

    [ForeignKey("GoodId")]
    public virtual Good Good { get; set; } = null!;

    [ForeignKey("WarehouseId")]
    public virtual Warehouse Warehouse { get; set; } = null!;
}
