namespace Station
{
    public interface IContainer
    {
        void SetPath(string path);
        T Load<T>();
        void Save<T>(T data);
        void Delete();
    }
}
