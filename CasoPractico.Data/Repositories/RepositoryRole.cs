using CasoPractico.Data.Models;
using CasoPractico.Model.DTOs;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;


namespace CasoPractico.Data.Repositories;
public interface IRoleRepository
{
    Task<IReadOnlyList<RoleDTO>> GetRolesAsync();
    Task<IReadOnlyList<UserRoleDTO>> GetUsersWithRoleAsync();
    Task<bool> UpsertUserRoleAsync(int userId, int roleId);
    Task<bool> UserExistsAsync(int userId);
    Task<bool> RoleExistsAsync(int roleId);
}

public class RepositoryRole : IRoleRepository
{
    private readonly string _cs;
    public RepositoryRole(IConfiguration cfg)
    {
        _cs = cfg.GetConnectionString("DefaultConnection")
              ?? throw new InvalidOperationException("DefaultConnection not found");
    }

    public async Task<IReadOnlyList<RoleDTO>> GetRolesAsync()
    {
        var list = new List<RoleDTO>();
        await using var con = new SqlConnection(_cs);
        await con.OpenAsync();

        var cmd = new SqlCommand(@"SELECT RoleId, RoleName FROM Roles ORDER BY RoleName", con);
        await using var rd = await cmd.ExecuteReaderAsync();
        while (await rd.ReadAsync())
        {
            list.Add(new RoleDTO
            {
                RoleId = rd.GetInt32(0),
                RoleName = rd.GetString(1)
            });
        }
        return list;
    }

    public async Task<IReadOnlyList<UserRoleDTO>> GetUsersWithRoleAsync()
    {
        var list = new List<UserRoleDTO>();
        await using var con = new SqlConnection(_cs);
        await con.OpenAsync();

        var cmd = new SqlCommand(@"
                SELECT u.UserId, u.Username, u.Email, u.IsActive,
                       ur.RoleId, r.RoleName, ur.Description
                FROM Users u
                LEFT JOIN UserRoles ur ON ur.UserId = u.UserId
                LEFT JOIN Roles r ON r.RoleId = ur.RoleId
                ORDER BY u.Username;", con);

        await using var rd = await cmd.ExecuteReaderAsync();
        while (await rd.ReadAsync())
        {
            list.Add(new UserRoleDTO
            {
                UserId = rd.GetInt32(0),
                Username = rd.GetString(1),
                Email = rd.GetString(2),
                IsActive = rd.GetBoolean(3),
                RoleId = rd.IsDBNull(4) ? (int?)null : rd.GetInt32(4),
                RoleName = rd.IsDBNull(5) ? null : rd.GetString(5),
                Description = rd.IsDBNull(6) ? null : rd.GetString(6)
            });
        }
        return list;
    }

    public async Task<bool> UserExistsAsync(int userId)
    {
        await using var con = new SqlConnection(_cs);
        await con.OpenAsync();
        var cmd = new SqlCommand("SELECT 1 FROM Users WHERE UserId = @u", con);
        cmd.Parameters.Add(new SqlParameter("@u", SqlDbType.Int) { Value = userId });
        var x = await cmd.ExecuteScalarAsync();
        return x != null;
    }

    public async Task<bool> RoleExistsAsync(int roleId)
    {
        await using var con = new SqlConnection(_cs);
        await con.OpenAsync();
        var cmd = new SqlCommand("SELECT 1 FROM Roles WHERE RoleId = @r", con);
        cmd.Parameters.Add(new SqlParameter("@r", SqlDbType.Int) { Value = roleId });
        var x = await cmd.ExecuteScalarAsync();
        return x != null;
    }

    public async Task<bool> UpsertUserRoleAsync(int userId, int roleId)
    {
        await using var con = new SqlConnection(_cs);
        await con.OpenAsync();
        await using var tx = await con.BeginTransactionAsync();

        try
        {
            var del = new SqlCommand("DELETE FROM UserRoles WHERE UserId = @u", con, (SqlTransaction)tx);
            del.Parameters.Add(new("@u", SqlDbType.Int) { Value = userId });
            await del.ExecuteNonQueryAsync();

            var ins = new SqlCommand(
                "INSERT INTO UserRoles(UserId, RoleId, Description) VALUES(@u, @r, NULL)",
                con, (SqlTransaction)tx);
            ins.Parameters.Add(new("@u", SqlDbType.Int) { Value = userId });
            ins.Parameters.Add(new("@r", SqlDbType.Int) { Value = roleId });
            await ins.ExecuteNonQueryAsync();

            await tx.CommitAsync();
            return true;
        }
        catch
        {
            await tx.RollbackAsync();
            return false;
        }
    }
}
