using CasoPractico.Data.Repositories;
using System.Net.Sockets;
using System.Text;
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
        var current = await repositoryTask.FindAsync(Task.Id);
        if (current == null) return false;

        var createdAtUtc = (current.CreatedAt?.ToUniversalTime()) ?? DateTime.UtcNow;
        if (current.Approved == false && Task.Approved == true)
        {
            var hours = (DateTime.UtcNow - createdAtUtc).TotalHours;
            if (hours > 24)
                return false; 
        }

        current.Name = string.IsNullOrWhiteSpace(Task.Name) ? current.Name : Task.Name;
        current.Description = Task.Description ?? current.Description;

        if (string.IsNullOrWhiteSpace(Task.Status))
            current.Status = string.IsNullOrWhiteSpace(current.Status) ? "Pending" : current.Status;
        else
            current.Status = Task.Status;

        current.DueDate = Task.DueDate == default ? current.DueDate : Task.DueDate;
        current.CreatedAt = Task.CreatedAt ?? current.CreatedAt;

        current.Approved = Task.Approved ?? current.Approved;

        return await repositoryTask.UpdateAsync(current);
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

