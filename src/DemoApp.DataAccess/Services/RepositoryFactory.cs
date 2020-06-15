namespace DemoApp.DataAccess.Services
{
    public interface IRepositoryFactory
    {
        IDataRepository<T> GetRepository<T>() where T : class;
    }
}
