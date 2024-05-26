using System.Collections.ObjectModel;
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
            IsLoading = false;
        }
    }

    [RelayCommand]
    public async Task Refresh()
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
}