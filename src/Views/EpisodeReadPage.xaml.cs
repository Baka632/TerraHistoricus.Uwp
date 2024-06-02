// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

using Microsoft.Toolkit.Uwp.UI.Controls;
using Microsoft.Toolkit.Uwp.UI.Extensions;

namespace TerraHistoricus.Uwp.Views;

[INotifyPropertyChanged]
/// <summary>
/// 可用于自身或导航至 Frame 内部的空白页。
/// </summary>
public sealed partial class EpisodeReadPage : Page
{
    [ObservableProperty]
    private ScrollViewer listViewerScrollViewer;

    public EpisodeReadViewModel ViewModel { get; } = new EpisodeReadViewModel();

    public EpisodeReadPage()
    {
        this.InitializeComponent();
        HideComicInfoStoryboard.Begin();
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
        if (ViewModel.ErrorVisibility != Visibility.Visible)
        {
            ShowComicInfoStoryboard.Begin();
        }
    }

    private void OnEpisodeListViewLoaded(object sender, RoutedEventArgs e)
    {
        EpisodeListView.SelectedIndex = EpisodeListView.Items.IndexOf(ViewModel.CurrentEpisodeInfo);
    }

    private void OnChromeGridTapped(object sender, TappedRoutedEventArgs e)
    {
        HideComicInfoStoryboard.Begin();
    }

    private void OnComicImageFailed(object sender, ImageExFailedEventArgs e)
    {
        ImageEx imageEx = (ImageEx)sender;

        if (imageEx.DataContext is PageDetailWrapper wrapper)
        {
            wrapper.IsSuccess = false;
        }
    }

    private async void OnPageContentGridLoaded(object sender, RoutedEventArgs e)
    {
        Grid grid = (Grid)sender;

        if (grid.DataContext is PageDetailWrapper wrapper && wrapper.IsSuccess != true)
        {
            await wrapper.TryInitialize();
        }
    }

    private void OnComicImageListLoaded(object sender, RoutedEventArgs e)
    {
        ScrollViewer listviewScrollViewer = ComicImageList.FindDescendant<ScrollViewer>();
        if (listviewScrollViewer is not null)
        {
            listviewScrollViewer.MinZoomFactor = 0.5f;
            listviewScrollViewer.MaxZoomFactor = 2f;
            ListViewerScrollViewer = listviewScrollViewer;
        }
    }
}
