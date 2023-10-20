using CommunityToolkit.Maui.Sample.Entities;

namespace CommunityToolkit.Maui.Sample.Repositories;

// TODO: reload files colloction on background file change
public class ImageLibraryRepository
{
	// image collection by sha256 hash, key is sha256 hash, value is dictionary of images with same hash, deduping by file path
	Dictionary<string, Dictionary<string, ImageMetadata>> imageCollectionBySha256;

	public bool IsLoaded { get; private set; }

	public ImageLibraryRepository()
	{
		imageCollectionBySha256 = new Dictionary<string, Dictionary<string, ImageMetadata>>();
	}

	public ImageLibraryRepository(List<string> imagePaths)
	{
		imageCollectionBySha256 = new Dictionary<string, Dictionary<string, ImageMetadata>>();
		LoadImageCollection(imagePaths);
	}

	public void LoadImageCollection(List<string> imagePaths)
	{
		foreach (string imagePath in imagePaths)
		{
			var image = new ImageMetadata(imagePath, preloadImageMetadata: true);
			var keyHash = image.ImageHashSha256String;
			if (!imageCollectionBySha256.ContainsKey(keyHash))
			{
				imageCollectionBySha256.Add(keyHash, new Dictionary<string, ImageMetadata>());
			}
			var duplicateImages = imageCollectionBySha256[keyHash];
			if(!duplicateImages.ContainsKey(imagePath))
			{
				duplicateImages.Add(imagePath, image);
			}
		}
		IsLoaded = true;
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
}
