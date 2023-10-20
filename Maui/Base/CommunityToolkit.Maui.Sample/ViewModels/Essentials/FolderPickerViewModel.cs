using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Sample.Services;
using CommunityToolkit.Maui.Storage;
using CommunityToolkit.Mvvm.Input;

namespace CommunityToolkit.Maui.Sample.ViewModels.Essentials;

public partial class FolderPickerViewModel : BaseViewModel
{
	readonly IFolderPicker folderPicker;
	readonly ImageReaderService imageReaderService;

	public FolderPickerViewModel(IFolderPicker folderPicker)
	{
		this.folderPicker = folderPicker;
		this.imageReaderService = new ImageReaderService();
	}

	[RelayCommand]
	async Task PickFolder(CancellationToken cancellationToken)
	{
		var folderPickerResult = await folderPicker.PickAsync(cancellationToken);
		if (folderPickerResult.IsSuccessful)
		{
			var filesCount = Directory.EnumerateFiles(folderPickerResult.Folder.Path).Count();
			var path = folderPickerResult.Folder.Path;
			this.imageReaderService.SetDirectoryPath(path);
			await Toast.Make($"Folder picked: Name - {folderPickerResult.Folder.Name}, Path - {path}, Files count - {filesCount}", ToastDuration.Long).Show(cancellationToken);
		}
		else
		{
			await Toast.Make($"Folder is not picked, {folderPickerResult.Exception.Message}").Show(cancellationToken);
		}
	}

	[RelayCommand]
	async Task ListDuplicateImages(CancellationToken cancellationToken)
	{
		var displayText = string.Empty;
		try
		{
			var images = this.imageReaderService.GetDuplicateImages();
			List<string> firstDuplicateImage = images.First().Select(image => image.FilePath).ToList();
			displayText = string.Join(Environment.NewLine, firstDuplicateImage);
			displayText += $"{Environment.NewLine}Images count: {firstDuplicateImage.Count}";
		}
		catch (Exception e)
		{
			displayText = $"Error while listing images, {e.Message}";
		}
		await Toast.Make(displayText).Show(cancellationToken);
	}

	[RelayCommand]
	async Task PickFolderStatic(CancellationToken cancellationToken)
	{
		var folderResult = await FolderPicker.PickAsync("DCIM", cancellationToken);
		if (folderResult.IsSuccessful)
		{
			var filesCount = Directory.EnumerateFiles(folderResult.Folder.Path).Count();
			var path = folderResult.Folder.Path;
			this.imageReaderService.SetDirectoryPath(path);
			await Toast.Make($"Folder picked: Name - {folderResult.Folder.Name}, Path - {folderResult.Folder.Path}, Files count - {filesCount}", ToastDuration.Long).Show(cancellationToken);
		}
		else
		{
			await Toast.Make($"Folder is not picked, {folderResult.Exception.Message}").Show(cancellationToken);
		}
	}

	[RelayCommand]
	async Task PickFolderInstance(CancellationToken cancellationToken)
	{
		var folderPickerInstance = new FolderPickerImplementation();
		try
		{
			var folderPickerResult = await folderPickerInstance.PickAsync(cancellationToken);
			folderPickerResult.EnsureSuccess();
			var path = folderPickerResult.Folder.Path;
			this.imageReaderService.SetDirectoryPath(path);
			await Toast.Make($"Folder picked: Name - {folderPickerResult.Folder.Name}, Path - {path}", ToastDuration.Long).Show(cancellationToken);
#if IOS || MACCATALYST
			folderPickerInstance.Dispose();
#endif
		}
		catch (Exception e)
		{
			await Toast.Make($"Folder is not picked, {e.Message}").Show(cancellationToken);
		}
	}
}