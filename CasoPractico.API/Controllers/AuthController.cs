using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace CasoPractico.API.Controllers
{
    [ApiController]
    [Route("api")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _cfg;
        public AuthController(IConfiguration cfg) => _cfg = cfg;

        public record LoginRequest(string Email, string Password);
        public record LoginUserDto(int UserId, string Username, string Email, bool IsActive, DateTime? LastLogin);
        public record RoleDto(int RoleId, string RoleName);

        [HttpPost("login")]
        public async Task<ActionResult<LoginUserDto>> Login([FromBody] LoginRequest req)
        {
            if (string.IsNullOrWhiteSpace(req.Email) || string.IsNullOrWhiteSpace(req.Password))
                return BadRequest("Email and password are required");

            if (req.Password != "admin") return Unauthorized("Invalid credentials");

            var cs = _cfg.GetConnectionString("DefaultConnection");
            await using var con = new SqlConnection(cs);
            await con.OpenAsync();

            var cmd = new SqlCommand(@"
                SELECT TOP 1 UserId, Username, Email, IsActive, LastLogin
                FROM Users
                WHERE Email = @e", con);
            cmd.Parameters.Add(new SqlParameter("@e", SqlDbType.NVarChar, 255) { Value = req.Email });

            await using var rd = await cmd.ExecuteReaderAsync();
            if (!await rd.ReadAsync()) return Unauthorized("User not found");

            var isActive = rd.GetBoolean(rd.GetOrdinal("IsActive"));
            if (!isActive) return Unauthorized("User inactive");

            var user = new LoginUserDto(
                rd.GetInt32(rd.GetOrdinal("UserId")),
                rd.GetString(rd.GetOrdinal("Username")),
                rd.GetString(rd.GetOrdinal("Email")),
                isActive,
                rd.IsDBNull(rd.GetOrdinal("LastLogin")) ? null : rd.GetDateTime(rd.GetOrdinal("LastLogin"))
            );

            return Ok(user);
        }

        [HttpGet("users/{userId:int}/roles")]
        public async Task<ActionResult<IEnumerable<RoleDto>>> GetUserRoles(int userId)
        {
            var cs = _cfg.GetConnectionString("DefaultConnection");
            await using var con = new SqlConnection(cs);
            await con.OpenAsync();

            var cmd = new SqlCommand(@"
                SELECT r.RoleId, r.RoleName
                FROM UserRoles ur
                JOIN Roles r ON r.RoleId = ur.RoleId
                WHERE ur.UserId = @uid", con);
            cmd.Parameters.Add(new SqlParameter("@uid", SqlDbType.Int) { Value = userId });

            var list = new List<RoleDto>();
            await using var rd = await cmd.ExecuteReaderAsync();
            while (await rd.ReadAsync())
            {
                list.Add(new RoleDto(
                    rd.GetInt32(rd.GetOrdinal("RoleId")),
                    rd.GetString(rd.GetOrdinal("RoleName"))
                ));
            }
            return Ok(list);
        }
    }
}
