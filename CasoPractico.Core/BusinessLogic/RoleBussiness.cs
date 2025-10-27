using CasoPractico.Data.Repositories;
using CasoPractico.Model.DTOs;

namespace CasoPractico.Core.BusinessLogic

{
    public interface IRoleBusiness
    {
        Task<IReadOnlyList<RoleDTO>> GetRolesAsync();
        Task<IReadOnlyList<UserRoleDTO>> GetUsersWithRoleAsync();
        Task<bool> UpsertUserRoleAsync(int userId, int roleId);
    }

    public class RoleBusiness : IRoleBusiness
    {
        private readonly IRoleRepository _repo;
        public RoleBusiness(IRoleRepository repo) => _repo = repo;

        public Task<IReadOnlyList<RoleDTO>> GetRolesAsync()
            => _repo.GetRolesAsync();

        public Task<IReadOnlyList<UserRoleDTO>> GetUsersWithRoleAsync()
            => _repo.GetUsersWithRoleAsync();

        public async Task<bool> UpsertUserRoleAsync(int userId, int roleId)
        {
            if (!await _repo.UserExistsAsync(userId)) return false;
            if (!await _repo.RoleExistsAsync(roleId)) return false;

            return await _repo.UpsertUserRoleAsync(userId, roleId);
        }
    }
}