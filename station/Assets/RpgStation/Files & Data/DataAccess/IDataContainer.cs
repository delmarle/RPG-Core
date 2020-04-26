namespace Station
{
    public interface IDataContainer
    {
        void SetPath(string path);
        T Load<T>();
        void Save<T>(T data);
        void Delete();
    }
}
