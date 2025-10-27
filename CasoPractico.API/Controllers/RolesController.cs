using CasoPractico.Core.BusinessLogic;
using CasoPractico.Data.Models;
using CasoPractico.Model.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace CasoPractico.API.Controllers

{
    [ApiController]
    [Route("api")]
    public class RolesController : ControllerBase
    {
        private readonly IRoleBusiness _biz;

        public RolesController(IRoleBusiness biz)
        {
            _biz = biz;
        }

        // GET api/roles
        [HttpGet("roles")]
        [ProducesResponseType(typeof(IEnumerable<RoleDTO>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<RoleDTO>>> GetRoles()
        {
            var roles = await _biz.GetRolesAsync();
            return Ok(roles);
        }

        // GET api/users-with-role
        [HttpGet("users-with-role")]
        [ProducesResponseType(typeof(IEnumerable<UserRoleDTO>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<UserRoleDTO>>> GetUsersWithRole()
        {
            var users = await _biz.GetUsersWithRoleAsync();
            return Ok(users);
        }

        public record AssignRoleRequest(int RoleId);

        // PUT api/users/{userId}/role
        [HttpPut("users/{userId:int}/role")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AssignRole(int userId, [FromBody] AssignRoleRequest req)
        {
            if (req is null || req.RoleId <= 0)
                return BadRequest("RoleId requerido.");

            var ok = await _biz.UpsertUserRoleAsync(userId, req.RoleId);
            if (!ok)
                return BadRequest("Usuario o rol inválido, o no fue posible actualizar.");

            return NoContent();
        }
    }
}
