namespace Station
{
	public interface IStorageAccess
	{
		void Write(string content, string path);
		string Read(string path);
        void Delete(string path);
	}
}