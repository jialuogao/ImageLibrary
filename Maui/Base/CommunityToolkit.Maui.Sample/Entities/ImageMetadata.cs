using CommunityToolkit.Maui.Sample.Utilities;
using SixLabors.ImageSharp.Formats;
using Image = SixLabors.ImageSharp.Image;

namespace CommunityToolkit.Maui.Sample.Entities;

public class ImageMetadata
{
	public string FilePath { get; set; }

	public string FileName
	{
		get
		{
			return Path.GetFileName(this.FilePath);
		}
	}

	public string FileExtension
	{
		get
		{
			return Path.GetExtension(this.FilePath);
		}
	}

	long? fileSizeInByte = null;

	public long FileSizeInByte
	{
		get
		{
			if (this.fileSizeInByte == null)
			{
				fileSizeInByte = this.ComputeFileSize();
			}
			return fileSizeInByte.Value;
		}
		private set
		{
			this.fileSizeInByte = value;
		}
	}

	byte[]? imageHashSha256 = null;

	public byte[] ImageHashSha256
	{
		get
		{
			if (this.imageHashSha256 == null)
			{
				this.ComputeMetadata();
			}
			return imageHashSha256!;
		}
		private set
		{
			this.imageHashSha256 = value;
		}
	}

	public string ImageHashSha256String
	{
		get
		{
			return Convert.ToBase64String(this.ImageHashSha256);
		}
	}

	int? imageWidth = null;

	public int ImageWidth
	{
		get
		{
			if (this.imageWidth == null)
			{
				this.LoadImageMetadata();
			}
			return imageWidth!.Value;
		}
		private set
		{
			this.imageWidth = value;
		}
	}

	int? imageHeight = null;

	public int ImageHeight
	{
		get
		{
			if (this.imageHeight == null)
			{
				this.LoadImageMetadata();
			}
			return imageHeight!.Value;
		}
		private set
		{
			this.imageHeight = value;
		}
	}

	Image? image = null;

	public Image Image
	{
		get
		{
			if (this.image == null)
			{
				this.image = Image.Load(this.FilePath);
			}
			return image;
		}
	}

	IImageFormat imageFormat => this.Image.Metadata.DecodedImageFormat!;

	byte[]? bytes = null;

	public byte[] Bytes
	{
		get
		{
			if (this.bytes == null)
			{
				this.bytes = ImageUtils.GetImageBytesFromImage(this.Image, imageFormat);
			}
			return bytes;
		}
	}

	public ImageMetadata(string filePath, bool preloadImageMetadata)
	{
		this.FilePath = filePath;
		if (preloadImageMetadata)
		{
			this.ComputeMetadata();
		}
	}

	public void ComputeMetadata()
	{
		using var stream = File.OpenRead(this.FilePath);
		this.FileSizeInByte = stream.Length;
		this.ImageHashSha256 = ImageUtils.GetImageHashSHA256(stream);
		LoadImageMetadata();
	}

	public void LoadImageMetadata()
	{
		this.ImageWidth = Image.Width;
		this.ImageHeight = Image.Height;
	}

	protected long ComputeFileSize()
	{
		return new FileInfo(this.FilePath).Length;
	}
}
