using System.ComponentModel.DataAnnotations;

namespace CustomMVC.ViewModels.Users
{
    public class CreateUserViewModel
    {
        [Required(ErrorMessage = "Поле 'Имя' обязательно для заполнения.")]
        [Display(Name = "Имя")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Имя должно содержать от 3 до 50 символов.")]
        [RegularExpression(@"^[a-zA-Z0-9_.-]*$", ErrorMessage = "Имя может содержать только буквы, цифры, точки, подчеркивания и дефисы.")]
        public string UserName { get; set; }
        [EmailAddress(ErrorMessage = "Некорректный адрес")]
        public string Email { get; set; }
        [Display(Name = "Пароль")]
        public string Password { get; set; }
        [Display(Name = "Дата регистрации")]
        [DataType(DataType.Date)]
        public DateTime RegistrationDate { get; set; }

        [Display(Name = "Роль")]
        public string UserRole { get; set; }
        public CreateUserViewModel()
        {
            UserRole = "user";
            RegistrationDate = DateTime.Now.Date;
        }
    }
}
