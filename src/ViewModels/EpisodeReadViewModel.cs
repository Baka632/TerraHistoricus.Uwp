using System.Net.Http;
using System.Threading;
using Microsoft.Toolkit.Uwp;
using Microsoft.Toolkit.Collections;

namespace TerraHistoricus.Uwp.ViewModels;

public sealed partial class EpisodeReadViewModel : ObservableObject
{
    [ObservableProperty]
    private bool showEpisodeInfo;
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
    private EpisodeInfo currentEpisodeInfo;
    [ObservableProperty]
    private IncrementalLoadingCollection<EpisodePageSource, PageDetail> pages;

    public async Task Initialize(ComicDetail comicDetail, EpisodeInfo episodeInfo)
    {
        IsLoading = true;
        ErrorVisibility = Visibility.Collapsed;

        try
        {
            if (MemoryCacheHelper<EpisodeDetail>.Default.TryGetData(episodeInfo.Cid, out EpisodeDetail detail))
            {
                CurrentEpisodeDetail = detail;
            }
            else
            {
                CurrentEpisodeDetail = await EpisodeAndPageService.GetEpisodeDetailAsync(comicDetail.Cid, episodeInfo.Cid);
                MemoryCacheHelper<EpisodeDetail>.Default.Store(episodeInfo.Cid, CurrentEpisodeDetail);
            }

            if (MemoryCacheHelper<IncrementalLoadingCollection<EpisodePageSource, PageDetail>>.Default.TryGetData(episodeInfo.Cid, out IncrementalLoadingCollection<EpisodePageSource, PageDetail> pages))
            {
                Pages = pages;
            }
            else
            {
                EpisodePageSource source = new(comicDetail, episodeInfo, CurrentEpisodeDetail);
                Pages = new IncrementalLoadingCollection<EpisodePageSource, PageDetail>(source, 5);

                MemoryCacheHelper<IncrementalLoadingCollection<EpisodePageSource, PageDetail>>.Default.Store(episodeInfo.Cid, Pages);
            }

            CurrentComic = comicDetail;
            CurrentEpisodeInfo = episodeInfo;
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

public sealed class EpisodePageSource(ComicDetail comicDetail, EpisodeInfo episodeInfo, EpisodeDetail episodeDetail) : IIncrementalSource<PageDetail>
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
                    PageDetail pageDetail = await EpisodeAndPageService.GetPageDetailAsync(comicDetail.Cid, episodeInfo.Cid, i);
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
                    PageDetail pageDetail = await EpisodeAndPageService.GetPageDetailAsync(comicDetail.Cid, episodeInfo.Cid, i);
                    _pages.Add(pageDetail);
                }
                isEnd = true;
            }
            else
            {
                for (int i = _pages.Count + 1; i <= totalLoadCount; i++)
                {
                    PageDetail pageDetail = await EpisodeAndPageService.GetPageDetailAsync(comicDetail.Cid, episodeInfo.Cid, i);
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