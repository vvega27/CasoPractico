using CasoPractico.Model.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly HttpClient _httpClient;


        public HomeController(IHttpClientFactory factory)
        {
            _httpClient = factory.CreateClient("CasoPractico");
        }

        public async Task<IActionResult> Index()
        {
            List<TaskDTO>? tasks = [];
            try
            {
                tasks = await _httpClient.GetFromJsonAsync<List<TaskDTO>>("api/tasks");
            }
            catch (Exception ex)
            {
                TempData["ApiError"] = $"No pude contactar el API: {ex.Message}";
            }

            return View(tasks);
        }
    }
}
