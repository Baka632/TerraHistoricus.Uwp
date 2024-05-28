using System.Net.Http;

namespace TerraHistoricus.Uwp.ViewModels;

public sealed partial class RecommendViewModel : ObservableObject
{
    [ObservableProperty]
    private bool isLoading = true;
    [ObservableProperty]
    private bool isRefreshing = false;
    [ObservableProperty]
    private Visibility errorVisibility = Visibility.Collapsed;
    [ObservableProperty]
    private ErrorInfo errorInfo;
    [ObservableProperty]
    private RecommendComicInfo currentRecommendInfo = new RecommendComicInfo() with
    {
        Authors = [],
        Keywords = [],
    };
    [ObservableProperty]
    private IEnumerable<EpisodeUpdateInfo> currentUpdateInfos;

    public async Task Initialize()
    {
        IsLoading = true;
        ErrorVisibility = Visibility.Collapsed;

        try
        {
            if (MemoryCacheHelper<RecommendComicInfo>.Default.TryGetData(CommonValues.RecommendComicInfoCacheKey, out RecommendComicInfo info))
            {
                CurrentRecommendInfo = info;
            }
            else
            {
                CurrentRecommendInfo = await InfoService.GetRecommendComicAsync();
                MemoryCacheHelper<RecommendComicInfo>.Default.Store(CommonValues.RecommendComicInfoCacheKey, CurrentRecommendInfo);
            }

            if (MemoryCacheHelper<IEnumerable<EpisodeUpdateInfo>>.Default.TryGetData(CommonValues.EpisodeUpdateInfosCacheKey, out IEnumerable<EpisodeUpdateInfo> updateInfos))
            {
                CurrentUpdateInfos = updateInfos;
            }
            else
            {
                CurrentUpdateInfos = await InfoService.GetRecentUpdateEpisodeAsync();
                MemoryCacheHelper<IEnumerable<EpisodeUpdateInfo>>.Default.Store(CommonValues.EpisodeUpdateInfosCacheKey, CurrentUpdateInfos);
            }

            ErrorVisibility = Visibility.Collapsed;
        }
        catch (HttpRequestException ex)
        {
            ShowInternetError(ex);
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task Refresh()
    {
        IsRefreshing = true;
        ErrorVisibility = Visibility.Collapsed;
        try
        {
            CurrentRecommendInfo = await InfoService.GetRecommendComicAsync();
            CurrentUpdateInfos = await InfoService.GetRecentUpdateEpisodeAsync();

            ErrorVisibility = Visibility.Collapsed;
        }
        catch (HttpRequestException ex)
        {
            ShowInternetError(ex);
        }
        finally
        {
            IsRefreshing = false;
        }
    }

    [RelayCommand]
    private void NavigateToComicDetailPage(string comicCid)
    {
        ContentFrameNavigationHelper.Navigate(typeof(ComicDetailPage), comicCid, transitionInfo: CommonValues.DefaultTransitionInfo);
    }

    [RelayCommand]
    private async Task NavigateToEpisodeDetailPage(string comicCid)
    {
        await DisplayContentDialog("期数 CID", comicCid, "OK".GetLocalized());
    }

    private void ShowInternetError(HttpRequestException ex)
    {
        ErrorVisibility = Visibility.Visible;
        ErrorInfo = new ErrorInfo()
        {
            Title = "ErrorOccurred".GetLocalized(),
            Message = "InternetErrorMessage".GetLocalized(),
            Exception = ex
        };
    }

    private static async Task DisplayContentDialog(string title, string message, string primaryButtonText = "", string closeButtonText = "")
    {
        await UIThreadHelper.RunOnUIThread(async () =>
        {
            ContentDialog contentDialog = new()
            {
                Title = title,
                Content = message,
                PrimaryButtonText = primaryButtonText,
                CloseButtonText = closeButtonText
            };

            await contentDialog.ShowAsync();
        });
    }
}