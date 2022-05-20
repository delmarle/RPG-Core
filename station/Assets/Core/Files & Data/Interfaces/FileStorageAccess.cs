namespace Station
{
	public class FileStorageAccess : IStorageAccess
	{
		public void Write(string content, string path)
		{
            IoUtils.CreateFile(content, path);
		}

		public string Read(string path)
		{
            string data;

            IoUtils.LoadFile(path, out data);

            return data;
        }

        public void Delete(string path)
        {
            IoUtils.DeleteFileOrDirectory(path);
        }
    }
}