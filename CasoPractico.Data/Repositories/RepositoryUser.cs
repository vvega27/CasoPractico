﻿using User = CasoPractico.Data.Models.User;
using CasoPractico.Data.Repositories;




namespace CasoPractico.Data.Repositories;
public interface IRepositoryUser
{
    Task<bool> UpsertAsync(User entity, bool isUpdating);
    Task<bool> CreateAsync(User entity);
    Task<bool> DeleteAsync(User entity);
    Task<IEnumerable<User>> ReadAsync();
    Task<User> FindAsync(int id);
    Task<bool> UpdateAsync(User entity);
    Task<bool> UpdateManyAsync(IEnumerable<User> entities);
    Task<bool> ExistsAsync(User entity);
}

public class  RepositoryUser : RepositoryBase<User>, IRepositoryUser    
{
    
}
