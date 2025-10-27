using CasoPractico.Model.DTOs;
using CasoPractico.MVC.Filters;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace CasoPractico.MVC.Controllers
{
    public class RolesController : Controller
    {
        private readonly HttpClient _http;
        public RolesController(IHttpClientFactory f) => _http = f.CreateClient("CasoPractico");

        public async Task<IActionResult> Index()
        {
            try
            {
                var opt = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

                var r1 = await _http.GetAsync("api/users-with-role");
                var users = r1.IsSuccessStatusCode
                    ? (await JsonSerializer.DeserializeAsync<List<UserRoleDTO>>(await r1.Content.ReadAsStreamAsync(), opt)) ?? new()
                    : new();

                var r2 = await _http.GetAsync("api/roles");
                var roles = r2.IsSuccessStatusCode
                    ? (await JsonSerializer.DeserializeAsync<List<RoleDTO>>(await r2.Content.ReadAsStreamAsync(), opt)) ?? new()
                    : new();

                ViewBag.AvailableRoles = roles;
                return View(users);
            }
            catch (Exception ex)
            {
                TempData["ApiError"] = $"Error cargando Roles: {ex.Message}";
                ViewBag.AvailableRoles = new List<RoleDTO>();
                return View(new List<UserRoleDTO>());
            }
        }
    }
}
