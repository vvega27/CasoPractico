namespace CasoPractico.ServiceLocator.ServiceFactory
{
    public interface IServiceFactory
    {
        object Create(string key);
    }
}
