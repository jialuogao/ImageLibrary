using System.Security.Cryptography;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats;
using Image = SixLabors.ImageSharp.Image;

namespace CommunityToolkit.Maui.Sample.Utilities;

public static class ImageUtils
{
	public static byte[] GetImageHashSHA512(byte[] data)
	{
		using (var sha512 = SHA512.Create())
		{
			return sha512.ComputeHash(data);
		}
	}

	public static byte[] GetImageHashSHA256(byte[] data)
	{
		using (var sha256 = SHA256.Create())
		{
			return sha256.ComputeHash(data);
		}
	}

	public static byte[] GetImageHashSHA256(Stream data)
	{
		using (var sha256 = SHA256.Create())
		{
			return sha256.ComputeHash(data);
		}
	}

	public static bool IsAnimatedImage(Image image)
	{
		return image.Frames.Count > 1;
	}

	public static byte[] GetImageBytesFromImage(Image image, IImageFormat format)
	{
		IImageEncoder encoder;
		var outputStream = new MemoryStream();
		if (format.Name == "JPEG")
		{
			encoder = new JpegEncoder()
			{
				Quality = 100
			};
		}
		else
		{
			encoder = Configuration.Default.ImageFormatsManager.GetEncoder(format);
		}

		image.Save(outputStream, encoder);

		outputStream.Flush();
		outputStream.Seek(0, SeekOrigin.Begin);
		var bytes = new byte[outputStream.Length];
		outputStream.Read(bytes, 0, bytes.Length);
		return bytes;
	}

	public static (int newWidth, int newHeight, bool scaled) ScaleWithinMax(int currentWidth, int currentHeight, int maxWidth, int maxHeight)
	{
		if (currentWidth <= maxWidth && currentHeight <= maxHeight)
		{
			return (currentWidth, currentHeight, false);
		}

		var widthScale = maxWidth / (double)currentWidth;
		var heightScale = maxHeight / (double)currentHeight;
		var scale = Math.Min(widthScale, heightScale);
		return ((int)Math.Round(currentWidth * scale),
			(int)Math.Round(currentHeight * scale), true);
	}

	public static Image ChangeImageSize(Image image, int width, int height)
	{
		return CopyImage(image, 0, 0, image.Width, image.Height, width, height);
	}

	public static Image CopyImage(Image image, int srcX, int srcY, int srcWidth, int srcHeight, int destWidth, int destHeight)
	{
		return image.Clone(
		  i => i.Resize(
			  destWidth,
			  destHeight,
			  KnownResamplers.Lanczos8,
			  new Rectangle(srcX, srcY, srcWidth, srcHeight),
			  new Rectangle(0, 0, destWidth, destHeight),
			  true));
	}
}
