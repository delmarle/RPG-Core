using System;
using UnityEngine;

namespace Station
{
	public class DataDataContainer : IDataContainer
	{
        private string _storagePath;
        private readonly ISerializer _serializer;
		private readonly IStorageAccess _storageAccess;

		public DataDataContainer(ISerializer serializer, IStorageAccess storageAccess)
		{
			_serializer = serializer;
			_storageAccess = storageAccess;
		}

        public void SetPath(string path)
        {
            _storagePath = path;
        }

        public T Load<T>() 
        {
            T data;
            try
            {
                string serializedContent = _storageAccess.Read(_storagePath);
                data = _serializer.UnSerialize<T>(serializedContent);
            }
            catch (Exception)
            {
                data = default;
            }

            return data;
        }

        public void Save<T>(T data)
        {
            try
            {
                string serializedContent = _serializer.Serialize(data);
                _storageAccess.Write(serializedContent, _storagePath);
            }
            catch(Exception)
            {
                Debug.LogError("cant save");
            }
        }

        public void Delete()
        {
            _storageAccess.Delete(_storagePath);
        }
    }
}