using System.Net.Http;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TerraHistoricus.Uwp.Models;

public partial record PageDetailWrapper(string ComicCid, string EpisodeCid, int PageNumber) : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;
    private IAsyncRelayCommand _retryInitialzationCommand;
    private Exception _ErrorException;
    private bool _IsSuccess;
    private bool _IsLoading;
    private PageDetail _Model;

    public Exception ErrorException
    {
        get => _ErrorException;
        private set
        {
            _ErrorException = value;
            OnPropertiesChanged();
        }
    }

    public bool IsSuccess
    {
        get => _IsSuccess;
        set
        {
            _IsSuccess = value;
            OnPropertiesChanged();
        }
    }

    public bool IsLoading
    {
        get => _IsLoading;
        private set
        {
            _IsLoading = value;
            OnPropertiesChanged();
        }
    }

    public PageDetail Model
    {
        get => _Model;
        private set
        {
            _Model = value;
            OnPropertiesChanged();
        }
    }

    public IAsyncRelayCommand RetryInitialzationCommand => _retryInitialzationCommand ??= new AsyncRelayCommand(RetryInitialize);

    public async Task<bool> TryInitialize()
    {
        IsLoading = true;
		try
		{
            Model = await EpisodeAndPageService.GetPageDetailAsync(ComicCid, EpisodeCid, PageNumber);
            IsSuccess = true;
            return true;
        }
		catch (HttpRequestException ex)
		{
            ErrorException = ex;
            IsSuccess = false;
            return false;
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task RetryInitialize()
    {
        await TryInitialize();
    }

    /// <summary>
    /// 通知运行时属性已经发生更改
    /// </summary>
    /// <param name="propertyName">发生更改的属性名称,其填充是自动完成的</param>
    public async void OnPropertiesChanged([CallerMemberName] string propertyName = "")
    {
        await UIThreadHelper.RunOnUIThread(() => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)));
    }
}
