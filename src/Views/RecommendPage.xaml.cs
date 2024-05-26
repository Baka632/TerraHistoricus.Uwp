// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

using Windows.UI.Core;

namespace TerraHistoricus.Uwp.Views;

/// <summary>
/// 可用于自身或导航至 Frame 内部的空白页。
/// </summary>
public sealed partial class RecommendPage : Page
{
    private readonly CoreCursor buttonCursor = new(CoreCursorType.Hand, 0);
    private CoreCursor cursorBeforePointerEntered = null;
    public RecommendViewModel ViewModel { get; } = new RecommendViewModel();

    public RecommendPage()
    {
        this.InitializeComponent();
        HideRecommendInfoStoryboard.Begin();
    }

    private async void OnPageLoaded(object sender, RoutedEventArgs e)
    {
        await ViewModel.Initialize();
    }

    private void OnRecommendComicGridPointerEntered(object sender, PointerRoutedEventArgs e)
    {
        cursorBeforePointerEntered = Window.Current.CoreWindow.PointerCursor;
        Window.Current.CoreWindow.PointerCursor = buttonCursor;

        ShowRecommendInfoStoryboard.Begin();
    }

    private void OnRecommendComicGridPointerExited(object sender, PointerRoutedEventArgs e)
    {
        Window.Current.CoreWindow.PointerCursor = cursorBeforePointerEntered;

        HideRecommendInfoStoryboard.Begin();
    }
}
