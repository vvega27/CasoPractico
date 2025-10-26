namespace CasoPractico.ServiceLocator.Services.Contracts;

public interface IService<T>
{
    Task<IEnumerable<T>> GetDataAsync();
    Task<bool> DeleteDataAsync(string id);
    Task<bool> CreateDataAsync(string content);
    Task<bool> UpdateDataAsync(string id, string content);
}
