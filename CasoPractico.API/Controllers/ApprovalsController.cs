using CasoPractico.Model.DTOs;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace CasoPractico.API.Controllers
{
    public class ApprovalsController : Controller
    {
        private readonly HttpClient _http;
        public ApprovalsController(IHttpClientFactory factory)
        {
            _http = factory.CreateClient("CasoPractico");
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var resp = await _http.GetAsync("api/tasks");
                if (!resp.IsSuccessStatusCode)
                {
                    var body = await resp.Content.ReadAsStringAsync();
                    TempData["ApiError"] = $"API GET /api/tasks → {(int)resp.StatusCode} {resp.ReasonPhrase}. Body: {body}";
                    return View(new List<TaskDTO>());
                }

                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var tasks = await JsonSerializer.DeserializeAsync<List<TaskDTO>>(
                    await resp.Content.ReadAsStreamAsync(), options
                ) ?? new();

                var ordered = tasks
                    .OrderBy(t => t.Approved.HasValue)               
                    .ThenByDescending(t => t.Approved == true)      
                    .ThenBy(t => t.DueDate)
                    .ToList();

                return View(ordered);
            }
            catch (Exception ex)
            {
                TempData["ApiError"] = $"Fallo al contactar la API: {ex.Message}";
                return View(new List<TaskDTO>());
            }
        }
    }
}
