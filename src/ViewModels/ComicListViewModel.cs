using System.Collections.ObjectModel;
using System.Net.Http;
using System.Threading.Tasks;

namespace TerraHistoricus.Uwp.ViewModels;

public sealed partial class ComicListViewModel : ObservableObject
{
    [ObservableProperty]
    private bool isLoading = false;
    [ObservableProperty]
    private bool isRefreshing = false;
    [ObservableProperty]
    private Visibility errorVisibility = Visibility.Collapsed;
    [ObservableProperty]
    private ErrorInfo errorInfo;
    [ObservableProperty]
    private ObservableCollection<ComicInfo> comics;

    public async Task Initialize()
    {
        IsLoading = true;
        ErrorVisibility = Visibility.Collapsed;
        try
        {
            if (MemoryCacheHelper<ObservableCollection<ComicInfo>>.Default.TryGetData(CommonValues.ComicInfoCacheKey, out ObservableCollection<ComicInfo> infos))
            {
                Comics = infos;
            }
            else
            {
                IEnumerable<ComicInfo> comicInfos = await ComicService.GetAllComicAsync();
                Comics = new ObservableCollection<ComicInfo>(comicInfos);
                MemoryCacheHelper<ObservableCollection<ComicInfo>>.Default.Store(CommonValues.ComicInfoCacheKey, Comics);
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
    public async Task RefreshComics()
    {
        IsRefreshing = true;
        ErrorVisibility = Visibility.Collapsed;
        try
        {
            IEnumerable<ComicInfo> comicInfos = await ComicService.GetAllComicAsync();

            if (Comics is null || !Comics.SequenceEqual(comicInfos))
            {
                Comics = new ObservableCollection<ComicInfo>(comicInfos);
                MemoryCacheHelper<ObservableCollection<ComicInfo>>.Default.Store(CommonValues.ComicInfoCacheKey, Comics);
            }

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