using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Text.Json;

namespace CasoPractico.MVC.Controllers

{
    public class LoginController : Controller
    {
        private readonly HttpClient _http;
        public LoginController(IHttpClientFactory f) => _http = f.CreateClient("CasoPractico"); 

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Index() => View();    

        public record LoginRequest(string Email, string Password);
        public record LoginUserDto(int UserId, string Username, string Email, bool IsActive, DateTime? LastLogin);
        public record RoleDto(int RoleId, string RoleName);

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Index(string email, string password)
        {
            try
            {
                var resp = await _http.PostAsJsonAsync("api/login", new LoginRequest(email, password));
                if (!resp.IsSuccessStatusCode)
                {
                    TempData["LoginError"] = "Credenciales inválidas.";
                    return View(); 
                }

                var opt = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var user = await resp.Content.ReadFromJsonAsync<LoginUserDto>(opt);
                if (user is null) { TempData["LoginError"] = "Respuesta inválida del API."; return View(); }

                var rolesResp = await _http.GetAsync($"api/users/{user.UserId}/roles");
                var roles = rolesResp.IsSuccessStatusCode
                    ? (await rolesResp.Content.ReadFromJsonAsync<List<RoleDto>>(opt) ?? new())
                    : new List<RoleDto>();

                HttpContext.Session.SetString("Username", user.Username);
                HttpContext.Session.SetString("UserEmail", user.Email);
                HttpContext.Session.SetString("UserId", user.UserId.ToString());
                HttpContext.Session.SetString("RolesCsv", string.Join(",", roles.Select(r => r.RoleName)));

                return RedirectToAction("Index", "Home"); 
            }
            catch (Exception ex)
            {
                TempData["LoginError"] = $"Error contactando API: {ex.Message}";
                return View(); 
            }
        }

        [AllowAnonymous]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index"); 
        }
    }
}