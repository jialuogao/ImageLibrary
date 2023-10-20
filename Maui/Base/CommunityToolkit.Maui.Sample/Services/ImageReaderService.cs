using System.Collections.Generic;
using System.Linq;
using CommunityToolkit.Maui.Sample.Entities;
using CommunityToolkit.Maui.Sample.Repositories;

namespace CommunityToolkit.Maui.Sample.Services;
public partial class ImageReaderService
{
	string? directoryPath;
	ImageLibraryRepository? imageLibraryRepository;

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

	public void SetDirectoryPath(string directoryPath, bool preloadImages = false)
	{
		this.directoryPath = directoryPath;
		if (preloadImages)
		{
			List<string> imageFiles = this.GetImageFilePaths();
			this.imageLibraryRepository = new ImageLibraryRepository(imageFiles);
		}
		else
		{
			this.imageLibraryRepository = new ImageLibraryRepository();
		}
	}

	public List<string> GetImageFilePaths()
	{
		if (string.IsNullOrEmpty(this.directoryPath))
		{
			throw new InvalidOperationException("Directory path is not set");
		}
		
		return EnumerateFiles(this.supportedImageFormats.ToList());
	}

	public List<List<ImageMetadata>> GetDuplicateImages()
	{
		if(this.imageLibraryRepository == null)
		{
			throw new InvalidOperationException("Image library repository is not initialized");
		}

		if(!this.imageLibraryRepository.IsLoaded)
		{
			this.imageLibraryRepository.LoadImageCollection(this.GetImageFilePaths());
		}
		var imageCollection = this.imageLibraryRepository.GetImageCollection();
		var duplicateImages = imageCollection.Where(x => x.Value.Count > 1).Select(x => x.Value).ToList();
		return duplicateImages;
	}

	protected List<string> EnumerateFiles(List<string> imageFormats)
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
