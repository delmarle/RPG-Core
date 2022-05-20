
namespace Station
{
    public static class DataStorageGenerator
    {
        public static DataDataContainer GenerateDataStorage()
        {
            var jsonSerializer = new JsonSerializer();
            var fileStorageAccess = new FileStorageAccess();

            return new DataDataContainer(jsonSerializer, fileStorageAccess);
        }
    }
}