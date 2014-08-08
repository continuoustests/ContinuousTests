namespace AutoTest.Core.Configuration
{
    public interface IServiceLocator
    {
        T Locate<T>();
		T Locate<T>(string name);
        T[] LocateAll<T>();
    }
}