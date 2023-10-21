using CommunityToolkit.Maui.Sample.ViewModels.Essentials;
using Image = Microsoft.Maui.Controls.Image;

namespace CommunityToolkit.Maui.Sample.Pages.Essentials;

public partial class FolderPickerPage : BasePage<FolderPickerViewModel>
{
	public FolderPickerPage(FolderPickerViewModel viewModel) : base(viewModel)
	{
		InitializeComponent();
	}

	void HandleAddButtonClicked(object? sender, System.EventArgs e)
	{
		UniformItemsLayout_MaxColumns6.Children.Add(
			new Image
			{
				Margin = new Thickness(5),
				WidthRequest = 100,
				HeightRequest = 100,
				Aspect = Aspect.AspectFill,
				Source = ImageSource.FromFile(@"C:\src\ImageLibrary\Maui\Base\TestData.ignore\SustainabilityBadge.jpg"),
			});
	}
}