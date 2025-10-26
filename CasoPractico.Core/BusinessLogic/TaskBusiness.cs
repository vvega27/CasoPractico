using CasoPractico.Data.Repositories;
using System.Net.Sockets;
using Task= CasoPractico.Data.Models.Task;


namespace CasoPractico.Core.BusinessLogic;

public interface ITaskBusiness
{
    /// <summary>
    /// Deletes the Task associated with the Task id.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<bool> DeleteTaskAsync(int id);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<IEnumerable<Task>> GetTasks(int? id);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="Task"></param>
    /// <returns></returns>
    Task<bool> SaveTaskAsync(Task Task);
    /// <summary>
    /// 
    Task<bool> CreateTasksAsync(Task task);

}

public class TaskBusiness(IRepositoryTask repositoryTask) : ITaskBusiness
{
    /// </inheritdoc>
    public async Task<bool> SaveTaskAsync(Task Task)
    {
        // que tengan mas de 5 quantity
        // sabado o domingo solo puedo salvar de 8 a 12
        return await repositoryTask.UpdateAsync(Task);
    }

    /// </inheritdoc>
    public async Task<bool> DeleteTaskAsync(int id)
    {
        var Task = await repositoryTask.FindAsync(id);
        return await repositoryTask.DeleteAsync(Task);
    }

    /// </inheritdoc>
    public async Task<IEnumerable<Task>> GetTasks(int? id)
    {
        return id == null
            ? await repositoryTask.ReadAsync()
            : [await repositoryTask.FindAsync((int)id)];
    }
    /// </inheritdoc>
    public async Task<bool> CreateTasksAsync(Task task)
    {
        return await repositoryTask.CreateAsync(task);
    }
}

