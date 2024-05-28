// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

using Windows.Networking.Connectivity;

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

    private async void OnComicCoverImageLoaded(object sender, RoutedEventArgs e)
    {
        ConnectionCost costInfo = NetworkInformation.GetInternetConnectionProfile().GetConnectionCost();

        if (costInfo?.NetworkCostType is NetworkCostType.Fixed or NetworkCostType.Variable)
        {
            return;
        }

        Image image = (Image)sender;
        if (image.DataContext is ComicInfo info)
        {
            Uri fileCoverUri = await FileCacheHelper.GetComicCoverUriAsync(info);
            if (fileCoverUri is null)
            {
                await FileCacheHelper.StoreComicCoverAsync(info);
            }
        }
    }

    private void OnContentGridViewItemClicked(object sender, ItemClickEventArgs e)
    {
        ComicInfo data = (ComicInfo)e.ClickedItem;
        ContentFrameNavigationHelper.Navigate(typeof(ComicDetailPage), data.Cid, transitionInfo: CommonValues.DefaultTransitionInfo);
    }

    private async void OnPageLoaded(object sender, RoutedEventArgs e)
    {
        await ViewModel.Initialize();
    }
}
