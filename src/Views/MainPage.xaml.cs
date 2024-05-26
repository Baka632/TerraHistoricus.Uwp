using Windows.UI;
using Windows.UI.Core;

// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace TerraHistoricus.Uwp.Views;

/// <summary>
/// 可用于自身或导航至 Frame 内部的空白页。
/// </summary>
public sealed partial class MainPage : Page
{
    public MainViewModel ViewModel { get; } = new MainViewModel();

    public MainPage()
    {
        this.InitializeComponent();

        Window.Current.SetTitleBar(TitleBar);

        ContentFrameNavigationHelper = new NavigationHelper(ContentFrame);
        ContentFrameNavigationHelper.Navigate(typeof(RecommendPage));
        ContentFrame.Navigated += OnContentFrameNavigated;

        ChangeSelectedItemOfNavigationView();
        AutoSetMainPageBackground();
        ConfigureTitleBar();
    }

    ~MainPage()
    {
        MainPageNavigationHelper.GoBackComplete -= OnMainPageGoBackComplete;
        MainPageNavigationHelper.NavigationComplete -= OnMainPageNavigationComplete;
        MainPageNavigationHelper = null;
    }

    private void OnMainPageLoaded(object sender, RoutedEventArgs e)
    {
        if (MainPageNavigationHelper is null)
        {
            MainPageNavigationHelper = new NavigationHelper(Frame, true);
            MainPageNavigationHelper.GoBackComplete += OnMainPageGoBackComplete;
        }
    }

    private void OnMainPageNavigationComplete(object sender, EventArgs e)
    {
        AppViewBackButtonVisibility backButtonVisibility = Frame.CanGoBack
                ? AppViewBackButtonVisibility.Visible
                : AppViewBackButtonVisibility.Collapsed;
        SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = backButtonVisibility;
    }

    private void OnMainPageGoBackComplete(object sender, EventArgs arg)
    {
        AppViewBackButtonVisibility backButtonVisibility = Frame.CanGoBack
                ? AppViewBackButtonVisibility.Visible
                : AppViewBackButtonVisibility.Collapsed;
        SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = backButtonVisibility;
        StartTitleTextBlockAnimation(backButtonVisibility);
        ChangeSelectedItemOfNavigationView();
    }

    private void OnContentFrameNavigated(object sender, NavigationEventArgs e)
    {
        if (e.NavigationMode == NavigationMode.Back)
        {
            ChangeSelectedItemOfNavigationView();
        }
    }

    private void AutoSetMainPageBackground()
    {
        AppBackgroundMode mode;
        if (MicaHelper.IsSupported())
        {
            mode = AppBackgroundMode.Mica;
        }
        else if (AcrylicHelper.IsSupported())
        {
            mode = AppBackgroundMode.Acrylic;
        }
        else
        {
            mode = AppBackgroundMode.PureColor;
        }

        SetMainPageBackground(mode);
    }

    public bool SetMainPageBackground(AppBackgroundMode mode)
    {
        switch (mode)
        {
            case AppBackgroundMode.Acrylic:
                return AcrylicHelper.TrySetAcrylicBrush(this);
            case AppBackgroundMode.Mica:
                // 设置 Mica 时，要将控件背景设置为透明
                Background = new SolidColorBrush(Colors.Transparent);
                return MicaHelper.TrySetMica(this);
            case AppBackgroundMode.PureColor:
            default:
                Background = Resources["ApplicationPageBackgroundThemeBrush"] as Brush;
                return true;
        }
    }
}
