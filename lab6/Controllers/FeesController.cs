using Microsoft.AspNetCore.Mvc;

namespace lab6.Controllers
{
    public class FeesController : Controller
    {
        // Метод для отображения страницы управления пошлинами
        [HttpGet]
        public IActionResult Index()
        {
            return View();  // Возвращаем представление Index.cshtml
        }
    }
}