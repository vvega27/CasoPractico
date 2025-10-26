using CasoPractico.Model.DTOs;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace CasoPractico.MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly HttpClient _http;

        public HomeController(IHttpClientFactory factory)
        {
            _http = factory.CreateClient("CasoPractico");
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var resp = await _http.GetAsync("api/Tasks");
                if (!resp.IsSuccessStatusCode)
                {
                    var body = await resp.Content.ReadAsStringAsync();
                    TempData["ApiError"] = $"API GET /api/Tasks → {(int)resp.StatusCode} {resp.ReasonPhrase}. Body: {body}";
                    return View(new List<TaskDTO>());
                }

                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var stream = await resp.Content.ReadAsStreamAsync();
                var tasks = await JsonSerializer.DeserializeAsync<List<TaskDTO>>(stream, options) ?? new List<TaskDTO>();

                tasks = tasks
                    .OrderByDescending(t => t.Approved == true) 
                    .ThenBy(t => t.DueDate)
                    .ToList();

                return View(tasks);
            }
            catch (Exception ex)
            {
                TempData["ApiError"] = $"Fallo al contactar la API: {ex.Message}";
                return View(new List<TaskDTO>());
            }
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
    }
}
