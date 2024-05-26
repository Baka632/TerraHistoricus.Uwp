using System.Net.Http;
using System.Security.Cryptography;

namespace TerraHistoricus.Uwp.ViewModels;

public sealed partial class ComicDetailViewModel : ObservableObject
{
    [ObservableProperty]
    private bool isLoading = false;
    [ObservableProperty]
    private Visibility errorVisibility = Visibility.Collapsed;
    [ObservableProperty]
    private ErrorInfo errorInfo;
    [ObservableProperty]
    private ComicDetail currentComicDetail = new ComicDetail() with
    {
        Authors = [],
        Keywords = [],
    };

    public async Task Initialize(string comicCid)
    {
        IsLoading = true;
        ErrorVisibility = Visibility.Collapsed;

        try
        {
            CurrentComicDetail = await ComicService.GetComicDetailAsync(comicCid);

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