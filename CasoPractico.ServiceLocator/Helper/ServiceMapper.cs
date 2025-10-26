using CasoPractico.Model.DTOs;
using CasoPractico.ServiceLocator.Services.Contracts;

namespace CasoPractico.ServiceLocator.Helper;

public interface IServiceMapper
{
    Task<IService<T>> GetServiceAsync<T>(string name);
}

public class ServiceMapper : IServiceMapper
{
    private readonly IServiceProvider serviceProvider;

    public ServiceMapper(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;
    }

    public Task<IService<T>> GetServiceAsync<T>(string name)
    {
        var service = name.ToLower() switch
        {
            "tasks" => (IService<T>)serviceProvider.GetRequiredService<IService<TaskDTO>>(),
            _ => throw new ArgumentException($"Service not found for '{name}'")
        };

        return Task.FromResult(service);
    }
}

