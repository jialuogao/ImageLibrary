using CommunityToolkit.Maui.Views;
using Size = Microsoft.Maui.Graphics.Size;

namespace CommunityToolkit.Maui.Sample.Views.Popups;

public sealed class TransparentPopupCSharp : Popup
{
	public TransparentPopupCSharp(Size popupSize) : this()
	{
		Size = popupSize;
		Color = Colors.Transparent;
	}

	public TransparentPopupCSharp()
	{
		Content = new Frame
		{
			CornerRadius = 25,
			HeightRequest = 50,
			WidthRequest = 50
		};
	}
}