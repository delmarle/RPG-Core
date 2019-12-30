
namespace Station
{
    public static class DataStorageGenerator
    {
        public static DataContainer GenerateDataStorage()
        {
            var jsonSerializer = new JsonSerializer();
            var fileStorageAccess = new FileStorageAccess();

            return new DataContainer(jsonSerializer, fileStorageAccess);
        }
    }
}