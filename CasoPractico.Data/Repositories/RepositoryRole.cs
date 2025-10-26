﻿using Role = CasoPractico.Data.Models.Role;
using CasoPractico.Data.Repositories;   



namespace CasoPractico.Data.Repositories;
public interface IRepositoryRole
{
    Task<bool> UpsertAsync(Role entity, bool isUpdating);
    Task<bool> CreateAsync(Role entity);
    Task<bool> DeleteAsync(Role entity);
    Task<IEnumerable<Role>> ReadAsync();
    Task<Role> FindAsync(int id);
    Task<bool> UpdateAsync(Role entity);
    Task<bool> UpdateManyAsync(IEnumerable<Role> entities);
    Task<bool> ExistsAsync(Role entity);
}

public class RepositoryRole : RepositoryBase<Role>, IRepositoryRole
{
}

