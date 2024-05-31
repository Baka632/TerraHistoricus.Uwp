// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace TerraHistoricus.Uwp.Views;

/// <summary>
/// 可用于自身或导航至 Frame 内部的空白页。
/// </summary>
public sealed partial class EpisodeReadPage : Page
{
    public EpisodeReadViewModel ViewModel { get; } = new EpisodeReadViewModel();

    public EpisodeReadPage()
    {
        this.InitializeComponent();
    }

    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);

        if (e.Parameter is ValueTuple<ComicDetail, EpisodeInfo> data)
        {
            await ViewModel.Initialize(data.Item1, data.Item2);
        }
        else if (e.Parameter is ValueTuple<string, string> cidData)
        {
            await ViewModel.Initialize(cidData.Item1, cidData.Item2);
        }
    }

    private async void OnEpisodeItemClick(object sender, ItemClickEventArgs e)
    {
        EpisodeInfo info = (EpisodeInfo)e.ClickedItem;

        EpisodeListFlyout.Hide();

        if (info != ViewModel.CurrentEpisodeInfo)
        {
            await ViewModel.Initialize(ViewModel.CurrentComic, info);
            //MainPageNavigationHelper.Navigate(typeof(EpisodeReadPage), (ViewModel.CurrentComic, info), new SuppressNavigationTransitionInfo());
        }
    }

    private void OnComicImageListTapped(object sender, TappedRoutedEventArgs e)
    {
        ViewModel.ShowEpisodeInfo = !ViewModel.ShowEpisodeInfo;
    }

    private void OnEpisodeListViewLoaded(object sender, RoutedEventArgs e)
    {
        EpisodeListView.SelectedIndex = EpisodeListView.Items.IndexOf(ViewModel.CurrentEpisodeInfo);
    }
}
