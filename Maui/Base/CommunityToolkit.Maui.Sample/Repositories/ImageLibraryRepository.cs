using CommunityToolkit.Maui.Sample.Entities;
using CommunityToolkit.Maui.Sample.Utilities;

namespace CommunityToolkit.Maui.Sample.Repositories;

// TODO: reload files colloction on background file change
public class ImageLibraryRepository
{
	string fileCacheName = "ImageLibraryRepository.yaml";
	// image collection by sha256 hash, key is sha256 hash, value is dictionary of images with same hash, deduping by file path
	Dictionary<string, Dictionary<string, ImageMetadata>> imageCollectionBySha256;
	Dictionary<string, ImageMetadata> imageCollectionByFilePath;
	string processPath = Path.GetDirectoryName(Environment.ProcessPath)!;

	string fileCachePath => Path.Combine(processPath, fileCacheName);

	public List<ImageMetadata> AllImageMetadata => imageCollectionByFilePath.Values.ToList();

	public bool IsLoaded { get; private set; }

	public ImageLibraryRepository()
	{
		imageCollectionBySha256 = new Dictionary<string, Dictionary<string, ImageMetadata>>();
		imageCollectionByFilePath = new Dictionary<string, ImageMetadata>();
	}

	public ImageLibraryRepository(List<string> imagePaths)
	{
		imageCollectionBySha256 = new Dictionary<string, Dictionary<string, ImageMetadata>>();
		imageCollectionByFilePath = new Dictionary<string, ImageMetadata>();
		this.LoadImageCollection(imagePaths).Wait();
	}

	public async Task LoadImageCollection(List<string> imagePaths)
	{
		if (!await this.LoadImageCollectionFromFileAsync())
		{
			this.LoadImageCollectionFromPath(imagePaths);
		}
		else
		{
			// TODO: only load the difference
			this.LoadImageCollectionFromPath(imagePaths);
		}
	}

	protected void LoadImageCollectionFromPath(List<string> imagePaths)
	{
		var imageMetadataList = this.LoadImageMetadata(imagePaths);
		this.SetupImageCollection(imageMetadataList);
	}

	protected void SetupImageCollection(List<ImageMetadata> imageMetadataList)
	{
		this.LoadImageFilePathCollection(imageMetadataList);
		this.LoadImageSha256Collection(imageMetadataList);
		this.IsLoaded = true;
	}

	protected void LoadImageFilePathCollection(List<ImageMetadata> imageMetadataList)
	{
		foreach (var image in imageMetadataList)
		{
			var imagePath = image.FilePath;
			if (!imageCollectionByFilePath.ContainsKey(imagePath))
			{
				imageCollectionByFilePath.Add(imagePath, image);
			}
		}
	}

	protected void LoadImageSha256Collection(List<ImageMetadata> imageMetadataList)
	{
		foreach (var image in imageMetadataList)
		{
			var imagePath = image.FilePath;
			var keyHash = image.ImageHashSha256String;
			if (!imageCollectionBySha256.ContainsKey(keyHash))
			{
				imageCollectionBySha256.Add(keyHash, new Dictionary<string, ImageMetadata>());
			}
			var duplicateImages = imageCollectionBySha256[keyHash];
			if (!duplicateImages.ContainsKey(imagePath))
			{
				duplicateImages.Add(imagePath, image);
			}
		}
	}

	public List<ImageMetadata> LoadImageMetadata(List<string> imagePaths)
	{
		var images = new List<ImageMetadata>();
		foreach (string imagePath in imagePaths)
		{
			var image = new ImageMetadata(imagePath, preloadImageMetadata: true);
			images.Add(image);
		}
		return images;
	}

	public Dictionary<string, List<ImageMetadata>> GetImageCollection()
	{
		if (!IsLoaded)
		{
			throw new InvalidOperationException("Image collection is not loaded");
		}
		return imageCollectionBySha256.Select(x => new KeyValuePair<string, List<ImageMetadata>>(x.Key, x.Value.Values.ToList())).ToDictionary(x => x.Key, x => x.Value);
	}

	public List<ImageMetadata> GetSimilarImages(ImageMetadata imageMetadata)
	{
		if (!IsLoaded)
		{
			throw new InvalidOperationException("Image collection is not loaded");
		}
		var keyHash = imageMetadata.ImageHashSha256String;
		var similarImages = new List<ImageMetadata>();
		if (imageCollectionBySha256.ContainsKey(keyHash))
		{
			similarImages = imageCollectionBySha256[keyHash].Select(x => x.Value).ToList();
		}
		return similarImages;
	}

	public async Task<bool> SaveImageCollectionToFileAsync()
	{
		try
		{
			await SerializationUtils.WriteObjectAsYamlFileAsync(fileCachePath, AllImageMetadata);
			return true;
		}
		catch
		{
			return false;
		}
	}

	protected async Task<bool> LoadImageCollectionFromFileAsync()
	{
		try
		{
			var imageMetadataList = await SerializationUtils.LoadObjectFromYamlFileAsync<List<ImageMetadata>>(fileCachePath);
			this.SetupImageCollection(imageMetadataList);
			return true;
		}
		catch
		{
			return false;
		}
	}
}
