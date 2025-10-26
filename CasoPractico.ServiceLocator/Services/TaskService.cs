using CasoPractico.Architecture.Providers;
using CasoPractico.ServiceLocator.Services.Contracts;
using CasoPractico.Model.DTOs;
using CasoPractico.Architecture;

namespace CasoPractico.ServiceLocator.Services;

public interface ITaskService
{
    Task<IEnumerable<TaskDTO>> GetDataAsync();
    Task<bool> CreateDataAsync(string content);
    Task<bool> UpdateDataAsync(string id, string content);
    Task<bool> DeleteDataAsync(string id);
}

public class TaskService(IRestProvider restProvider, IConfiguration configuration)
    : IService<TaskDTO>, ITaskService
{
    public async Task<IEnumerable<TaskDTO>> GetDataAsync()
    {
        var url = configuration.GetStringFromAppSettings("APIS", "Task");
        var response = await restProvider.GetAsync(url, null);
        return await JsonProvider.DeserializeAsync<IEnumerable<TaskDTO>>(response);
    }

    public async Task<bool> CreateDataAsync(string content)
    {
        var url = configuration.GetStringFromAppSettings("APIS", "Task");
        var response = await restProvider.PostAsync(url, content);
        return bool.Parse(response);
    }

    public async Task<bool> UpdateDataAsync(string id, string content)
    {
        var url = configuration.GetStringFromAppSettings("APIS", "Task");
        var response = await restProvider.PutAsync(url, id, content);
        return bool.Parse(response);
    }

    public async Task<bool> DeleteDataAsync(string id)
    {
        var url = configuration.GetStringFromAppSettings("APIS", "Task");
        var response = await restProvider.DeleteAsync(url, id);
        return bool.Parse(response);
    }
}