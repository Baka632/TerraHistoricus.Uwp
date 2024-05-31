using System.Net.Http;

namespace TerraHistoricus.Uwp.ViewModels;

public sealed partial class ComicDetailViewModel : ObservableObject
{
    [ObservableProperty]
    private bool isLoading = false;
    [ObservableProperty]
    private Visibility errorVisibility = Visibility.Collapsed;
    [ObservableProperty]
    private ErrorInfo errorInfo;
    [NotifyPropertyChangedFor(nameof(IsMultipleEpisode))]
    [ObservableProperty]
    private ComicDetail currentComicDetail = new ComicDetail() with
    {
        Authors = [],
        Keywords = [],
    };

    public bool IsMultipleEpisode { get => CurrentComicDetail.Episodes is not null && CurrentComicDetail.Episodes.Count() > 1; }

    public async Task Initialize(string comicCid)
    {
        IsLoading = true;
        ErrorVisibility = Visibility.Collapsed;

        try
        {
            if (MemoryCacheHelper<ComicDetail>.Default.TryGetData(comicCid, out ComicDetail comicDetail))
            {
                CurrentComicDetail = comicDetail;
            }
            else
            {
                CurrentComicDetail = await ComicService.GetComicDetailAsync(comicCid);

                Uri fileCoverUri = await FileCacheHelper.GetComicCoverUriAsync(CurrentComicDetail);
                if (fileCoverUri != null)
                {
                    CurrentComicDetail = CurrentComicDetail with { CoverUri = fileCoverUri.ToString() };
                }

                MemoryCacheHelper<ComicDetail>.Default.Store(comicCid, CurrentComicDetail);
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
    private void NavigateToEpisodeReadPage(EpisodeInfo info)
    {
        MainPageNavigationHelper.Navigate(typeof(EpisodeReadPage), (CurrentComicDetail, info));
    }

    [RelayCommand]
    private void ReadLatestEpisode()
    {
        EpisodeInfo latestEpisode = CurrentComicDetail.Episodes.FirstOrDefault();
        MainPageNavigationHelper.Navigate(typeof(EpisodeReadPage), (CurrentComicDetail, latestEpisode));
    }

    [RelayCommand]
    private void ReadFirstEpisode()
    {
        EpisodeInfo firstEpisode = CurrentComicDetail.Episodes.LastOrDefault();
        MainPageNavigationHelper.Navigate(typeof(EpisodeReadPage), (CurrentComicDetail, firstEpisode));
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