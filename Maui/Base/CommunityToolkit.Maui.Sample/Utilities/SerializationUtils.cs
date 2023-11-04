using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace CommunityToolkit.Maui.Sample.Utilities;

public static class SerializationUtils
{
	public static async Task WriteObjectAsYamlFileAsync<T>(string dstFilePath, T objectToWrite)
	{
		var serializer = new SerializerBuilder()
			.WithNamingConvention(UnderscoredNamingConvention.Instance)
			.Build();
		var yaml = serializer.Serialize(objectToWrite);
		await File.WriteAllTextAsync(dstFilePath, yaml);
	}

	public static async Task<T> LoadObjectFromYamlFileAsync<T>(string srcFilePath)
	{
		var deserializer = new DeserializerBuilder()
			.WithNamingConvention(UnderscoredNamingConvention.Instance)
			.Build();
		var yaml = await File.ReadAllTextAsync(srcFilePath);
		return deserializer.Deserialize<T>(yaml);
	}
}
