﻿using System.ComponentModel.DataAnnotations;

namespace CustomMVC.ViewModels.Users
{
    public class UserViewModel
    {
        public string Id { get; set; }
        [Display(Name = "Имя")]
        public string UserName { get; set; }
        [EmailAddress(ErrorMessage = "Некорректный адрес")]
        public string Email { get; set; }
        [Display(Name = "Дата регистрации")]
        [DataType(DataType.Date)]
        public DateTime RegistrationDate { get; set; }
        [Display(Name = "Роль")]
        public string RoleName { get; set; }
    }
}
