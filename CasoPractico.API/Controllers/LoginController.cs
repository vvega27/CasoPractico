using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace CasoPractico.API.Controllers

{
    public record LoginRequest(string Username);
    public record LoginResponse(int UserId, string Username, string Email, bool IsActive);

    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly IConfiguration _cfg;
        public LoginController(IConfiguration cfg) => _cfg = cfg;

        [HttpPost]
        public async Task<ActionResult<LoginResponse>> Post([FromBody] LoginRequest req)
        {
            if (string.IsNullOrWhiteSpace(req.Username))
                return BadRequest("Username required");

            // Usa tu cadena de conexión (appsettings.json) — aquí un ejemplo:
            var connStr = _cfg.GetConnectionString("DefaultConnection");
            await using var con = new SqlConnection(connStr);
            await con.OpenAsync();

            var cmd = new SqlCommand(@"
                SELECT TOP 1 UserId, Username, Email, IsActive
                FROM Users
                WHERE Username = @u", con);
            cmd.Parameters.Add(new SqlParameter("@u", SqlDbType.NVarChar, 50) { Value = req.Username });

            await using var rdr = await cmd.ExecuteReaderAsync();
            if (!await rdr.ReadAsync())
                return Unauthorized();

            var isActive = rdr.GetBoolean(rdr.GetOrdinal("IsActive"));
            if (!isActive) return Unauthorized();

            var user = new LoginResponse(
                rdr.GetInt32(rdr.GetOrdinal("UserId")),
                rdr.GetString(rdr.GetOrdinal("Username")),
                rdr.GetString(rdr.GetOrdinal("Email")),
                isActive
            );

            return Ok(user);
        }
    }
}   
