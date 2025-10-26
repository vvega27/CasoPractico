using Task = CasoPractico.Data.Models.Task;
using CasoPractico.Data.Repositories;



namespace CasoPractico.Data.Repositories;
public interface IRepositoryTask
{
    Task<bool> UpsertAsync(Task entity, bool isUpdating);
    Task<bool> CreateAsync(Task entity);
    Task<bool> DeleteAsync(Task entity);
    Task<IEnumerable<Task>> ReadAsync();
    Task<Task> FindAsync(int id);
    Task<bool> UpdateAsync(Task entity);
    Task<bool> UpdateManyAsync(IEnumerable<Task> entities);
    Task<bool> ExistsAsync(Task entity);
}

public class RepositoryTask : RepositoryBase<Task>, IRepositoryTask
{
}