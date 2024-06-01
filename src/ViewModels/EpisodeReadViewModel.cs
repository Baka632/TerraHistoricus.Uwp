using System.Net.Http;
using System.Threading;
using Microsoft.Toolkit.Uwp;
using Microsoft.Toolkit.Collections;

namespace TerraHistoricus.Uwp.ViewModels;

public sealed partial class EpisodeReadViewModel : ObservableObject
{
    [ObservableProperty]
    private bool isLoading = false;
    [ObservableProperty]
    private Visibility errorVisibility = Visibility.Collapsed;
    [ObservableProperty]
    private ErrorInfo errorInfo;
    [ObservableProperty]
    private ComicDetail currentComic;
    [ObservableProperty]
    private EpisodeDetail currentEpisodeDetail;
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ToNextEpisodeCommand))]
    [NotifyCanExecuteChangedFor(nameof(ToPreviousEpisodeCommand))]
    private EpisodeInfo currentEpisodeInfo;
    [ObservableProperty]
    private IncrementalLoadingCollection<EpisodePageSource, PageDetail> pages;

    public bool HasNextEpisode => CurrentComic.Episodes is not null && CurrentComic.Episodes.FirstOrDefault() != CurrentEpisodeInfo;

    public bool HasPreviousEpisode => CurrentComic.Episodes is not null && CurrentComic.Episodes.LastOrDefault() != CurrentEpisodeInfo;

    public async Task Initialize(ComicDetail comicDetail, EpisodeInfo episodeInfo)
    {
        IsLoading = true;
        ErrorVisibility = Visibility.Collapsed;

        try
        {
            await InitializeCore(comicDetail, episodeInfo);
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

    public async Task Initialize(string comicCid, string episodeCid)
    {
        IsLoading = true;
        ErrorVisibility = Visibility.Collapsed;

        try
        {
            ComicDetail comicDetail;

            if (MemoryCacheHelper<ComicDetail>.Default.TryGetData(comicCid, out ComicDetail value))
            {
                comicDetail = value;
            }
            else
            {
                comicDetail = await ComicService.GetComicDetailAsync(comicCid);
                MemoryCacheHelper<ComicDetail>.Default.Store(comicCid, comicDetail);
            }

            EpisodeInfo episodeInfo = comicDetail.Episodes.FirstOrDefault(info => info.Cid == episodeCid);

            await InitializeCore(comicDetail, episodeInfo);
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

    public async Task InitializeCore(ComicDetail comicDetail, EpisodeInfo episodeInfo)
    {
        string episodeCid = episodeInfo.Cid;

        if (MemoryCacheHelper<EpisodeDetail>.Default.TryGetData(episodeCid, out EpisodeDetail detail))
        {
            CurrentEpisodeDetail = detail;
        }
        else
        {
            CurrentEpisodeDetail = await EpisodeAndPageService.GetEpisodeDetailAsync(comicDetail.Cid, episodeCid);
            MemoryCacheHelper<EpisodeDetail>.Default.Store(episodeCid, CurrentEpisodeDetail);
        }

        if (MemoryCacheHelper<IncrementalLoadingCollection<EpisodePageSource, PageDetail>>.Default.TryGetData(episodeCid, out IncrementalLoadingCollection<EpisodePageSource, PageDetail> pages))
        {
            Pages = pages;
        }
        else
        {
            EpisodePageSource source = new(comicDetail, episodeCid, CurrentEpisodeDetail);
            Pages = new IncrementalLoadingCollection<EpisodePageSource, PageDetail>(source, 5);

            MemoryCacheHelper<IncrementalLoadingCollection<EpisodePageSource, PageDetail>>.Default.Store(episodeCid, Pages);
        }

        CurrentComic = comicDetail;
        CurrentEpisodeInfo = episodeInfo;
    }

    [RelayCommand(CanExecute = nameof(HasNextEpisode))]
    private async Task ToNextEpisode()
    {
        List<EpisodeInfo> episodeList = CurrentComic.Episodes?.ToList();
        if (episodeList is null)
        {
            return;
        }

        int currentEpisodeIndex = episodeList.IndexOf(CurrentEpisodeInfo);
        if (currentEpisodeIndex == -1)
        {
            return;
        }

        if (currentEpisodeIndex - 1 >= 0)
        {
            EpisodeInfo nextEpisode = episodeList[currentEpisodeIndex - 1];
            await Initialize(CurrentComic, nextEpisode);
        }
    }

    [RelayCommand(CanExecute = nameof(HasPreviousEpisode))]
    private async Task ToPreviousEpisode()
    {
        List<EpisodeInfo> episodeList = CurrentComic.Episodes?.ToList();
        if (episodeList is null)
        {
            return;
        }

        int currentEpisodeIndex = episodeList.IndexOf(CurrentEpisodeInfo);
        if (currentEpisodeIndex == -1)
        {
            return;
        }

        if (currentEpisodeIndex + 1 < episodeList.Count)
        {
            EpisodeInfo previousEpisode = episodeList[currentEpisodeIndex + 1];
            await Initialize(CurrentComic, previousEpisode);
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

public sealed class EpisodePageSource(ComicDetail comicDetail, string episodeCid, EpisodeDetail episodeDetail) : IIncrementalSource<PageDetail>
{
    private readonly SemaphoreSlim _mutex = new(1);
    private bool isEnd = false;
    private readonly List<PageDetail> _pages = new(episodeDetail.PageInfos.Count);

    public async Task<IEnumerable<PageDetail>> GetPagedItemsAsync(int pageIndex, int pageSize, CancellationToken cancellationToken = default)
    {
        if (isEnd)
        {
            return [];
        }

        try
        {
            await _mutex.WaitAsync();

            int totalPageCount = episodeDetail.PageInfos.Count;
            int totalLoadCount = (pageIndex * pageSize) + pageSize;

            if (pageIndex == 0)
            {
                bool predicate = pageSize > totalPageCount;
                int pageCount = predicate ? totalPageCount : pageSize;

                for (int i = 1; i <= pageCount; i++)
                {
                    PageDetail pageDetail = await EpisodeAndPageService.GetPageDetailAsync(comicDetail.Cid, episodeCid, i);
                    _pages.Add(pageDetail);
                }

                if (predicate)
                {
                    isEnd = true;
                }
            }
            else if (totalLoadCount > totalPageCount)
            {
                for (int i = _pages.Count + 1; i <= totalPageCount; i++)
                {
                    PageDetail pageDetail = await EpisodeAndPageService.GetPageDetailAsync(comicDetail.Cid, episodeCid, i);
                    _pages.Add(pageDetail);
                }
                isEnd = true;
            }
            else
            {
                for (int i = _pages.Count + 1; i <= totalLoadCount; i++)
                {
                    PageDetail pageDetail = await EpisodeAndPageService.GetPageDetailAsync(comicDetail.Cid, episodeCid, i);
                    _pages.Add(pageDetail);
                }
            }

            return _pages.Skip(pageIndex * pageSize).Take(pageSize);
        }
        finally
        {
            _mutex.Release();
        }
    }
}