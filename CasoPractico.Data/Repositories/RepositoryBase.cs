﻿using CasoPractico.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace CasoPractico.Data.Repositories;

/// <summary>
/// Interface for basic repository operations.
/// </summary>
/// <typeparam name="T">The type of entity.</typeparam>
public interface IRepositoryBase<T>
{
    /// <summary>
    /// Method that updates the entity if exists otherwise inserts a new record
    /// </summary>
    /// <param name="entity">The entity to be deleted.</param>
    /// <param name="isUpdating">The indicator that tells if I need to update or create</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task<bool> UpsertAsync(T entity, bool isUpdating);

    /// <summary>
    /// Creates a new entity asynchronously.
    /// </summary>
    /// <param name="entity">The entity to be created.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a boolean indicating success.</returns>
    Task<bool> CreateAsync(T entity);

    /// <summary>
    /// Deletes an existing entity asynchronously.
    /// </summary>
    /// <param name="entity">The entity to be deleted.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a boolean indicating success.</returns>
    Task<bool> DeleteAsync(T entity);

    /// <summary>
    /// Reads all entities of type T asynchronously.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains a collection of entities.</returns>
    Task<IEnumerable<T>> ReadAsync();

    /// <summary>
    /// Finds an entity from the list of objects
    /// </summary>
    /// <param name="id">integer</param>
    /// <returns>Entity by id</returns>
    Task<T> FindAsync(int id);

    /// <summary>
    /// Updates an existing entity asynchronously.
    /// </summary>
    /// <param name="entity">The entity to be updated.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a boolean indicating success.</returns>
    Task<bool> UpdateAsync(T entity);

    /// <summary>
    /// Updates multiple entities asynchronously.
    /// </summary>
    /// <param name="entities">The collection of entities to be updated.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a boolean indicating success.</returns>
    Task<bool> UpdateManyAsync(IEnumerable<T> entities);

    /// <summary>
    /// Checks if an entity exists asynchronously.
    /// </summary>
    /// <param name="entity">The entity to check for existence.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a boolean indicating if the entity exists.</returns>
    Task<bool> ExistsAsync(T entity);
}

/// <summary>
/// Base class for repository operations.
/// </summary>
/// <typeparam name="T">Entity type.</typeparam>
public abstract class RepositoryBase<T> : IRepositoryBase<T> where T : class
{
    private readonly TaskDbContext _context;
    protected TaskDbContext DbContext => _context;
    protected DbSet<T> DbSet;

    /// <summary>
    /// Initializes a new instance of the <see cref="RepositoryBase{T}"/> class.
    /// </summary>
    public RepositoryBase()
    {
        _context = new TaskDbContext();
        DbSet<T> _sdbSet = _context.Set<T>();
    }

    public async Task<bool> UpsertAsync(T entity, bool isUpdating)
    {
        return isUpdating
            ? await UpdateAsync(entity)
            : await CreateAsync(entity);
    }

    /// <summary>
    /// Creates an entity of type T asynchronously.
    /// </summary>
    /// <param name="entity">The entity to be saved in the database.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a boolean indicating success.</returns>
    public async Task<bool> CreateAsync(T entity)
    {
        try
        {
            await _context.AddAsync(entity);
            return await SaveAsync();
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    /// <summary>
    /// Updates an existing entity of type T asynchronously.
    /// </summary>
    /// <param name="entity">The entity to be updated.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a boolean indicating success.</returns>
    public async Task<bool> UpdateAsync(T entity)
    {
        try
        {
            _context.Update(entity);
            return await SaveAsync();
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    /// <summary>
    /// Updates multiple entities of type T asynchronously.
    /// </summary>
    /// <param name="entities">The collection of entities to be updated.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a boolean indicating success.</returns>
    public async Task<bool> UpdateManyAsync(IEnumerable<T> entities)
    {
        try
        {
            _context.UpdateRange(entities);
            return await SaveAsync();
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    /// <summary>
    /// Deletes an entity of type T asynchronously.
    /// </summary>
    /// <param name="entity">The entity to be deleted.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a boolean indicating success.</returns>
    public async Task<bool> DeleteAsync(T entity)
    {
        try
        {
            _context.Remove(entity);
            return await SaveAsync();
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    /// <summary>
    /// Reads all entities of type T asynchronously.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains a collection of entities.</returns>
    public async Task<IEnumerable<T>> ReadAsync()
    {
        try
        {
            return await _context.Set<T>().ToListAsync();
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    /// <summary>
    /// Reads an entity of type T asynchronously.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains a collection of entities.</returns>
    public async Task<T> FindAsync(int id)
    {
        try
        {
            return await _context.Set<T>().FindAsync(id);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    /// <summary>
    /// Checks if an entity of type T exists asynchronously.
    /// </summary>
    /// <param name="entity">The entity to check for existence.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a boolean indicating if the entity exists.</returns>
    public async Task<bool> ExistsAsync(T entity)
    {
        try
        {
            var items = await ReadAsync();
            return items.Any(x => x.Equals(entity));
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    /// <summary>
    /// Saves changes to the database asynchronously.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains a boolean indicating success.</returns>
    protected async Task<bool> SaveAsync()
    {
        var result = await _context.SaveChangesAsync();
        return result > 0;
    }

}
