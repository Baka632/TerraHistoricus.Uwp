// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace TerraHistoricus.Uwp.Views;

/// <summary>
/// 可用于自身或导航至 Frame 内部的空白页。
/// </summary>
public sealed partial class ComicListPage : Page
{
    public ComicListViewModel ViewModel { get; } = new ComicListViewModel();

    public ComicListPage()
    {
        this.InitializeComponent();
        NavigationCacheMode = NavigationCacheMode.Enabled;
    }

    private void OnAlbumImageLoaded(object sender, RoutedEventArgs e)
    {

    }

    private void OnContentGridViewItemClicked(object sender, ItemClickEventArgs e)
    {

    }

    private async void OnPageLoaded(object sender, RoutedEventArgs e)
    {
        await ViewModel.Initialize();
    }
}
