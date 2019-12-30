namespace Station
{
	public interface ISerializer
	{
		string Serialize<T>(T data);
		T UnSerialize<T>(string serializedData);
	}
}