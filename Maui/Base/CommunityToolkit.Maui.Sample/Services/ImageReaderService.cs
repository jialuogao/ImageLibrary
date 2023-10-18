using System.Collections.Generic;
using System.Linq;

namespace CommunityToolkit.Maui.Sample.Services;
public partial class ImageReaderService
{
	string? directoryPath;

	HashSet<string> supportedImageFormats = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase)
	{
		".png",
		".jpg",
		".jpeg",
		".gif",
		".bmp",
		".tiff",
		".webp"
	};

	public ImageReaderService()
	{

	}

	public void SetDirectoryPath(string directoryPath)
	{
		this.directoryPath = directoryPath;
	}

	public IList<string> GetImageFiles()
	{
		if (string.IsNullOrEmpty(this.directoryPath))
		{
			throw new InvalidOperationException("Directory path is not set");
		}
		
		return EnumerateFiles(this.supportedImageFormats.ToList());
	}

	protected IList<string> EnumerateFiles(List<string> imageFormats)
	{
		if (string.IsNullOrEmpty(this.directoryPath))
		{
			throw new InvalidOperationException("Directory path is not set");
		}

		var files = new List<string>();

		foreach (string imageFormat in imageFormats)
		{
			if (this.supportedImageFormats.Contains(imageFormat))
			{
				files.AddRange(Directory.EnumerateFiles(this.directoryPath, $"*{imageFormat}", SearchOption.AllDirectories));
			}
			else
			{
				throw new NotSupportedException($"Image format {imageFormat} is not supported");
			}
		}

		return files;
	}
}
