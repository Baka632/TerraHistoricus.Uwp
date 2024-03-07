using Windows.ApplicationModel.Core;
using Windows.UI.Core;

namespace TerraHistoricus.Uwp.Views;

public partial class MainPage
{
    private bool IsTitleBarTextBlockForwardBegun = false;
    private bool IsFirstRun = true;

    private void ConfigureTitleBar()
    {
        if (EnvironmentHelper.IsWindowsMobile)
        {
            TitleBarTextBlock.Visibility = Visibility.Collapsed;
        }
        else
        {
            TitleBarHelper.TitleBarVisibilityChanged += OnTitleBarVisibilityChanged;
        }
    }

    private void OnTitleBarVisibilityChanged(object sender, CoreApplicationViewTitleBar bar)
    {
        if (bar.IsVisible)
        {
            StartTitleBarAnimation(Visibility.Visible);
        }
        else
        {
            StartTitleBarAnimation(Visibility.Collapsed);
        }
    }

    private void StartTitleTextBlockAnimation(AppViewBackButtonVisibility buttonVisibility)
    {
        switch (buttonVisibility)
        {
            case AppViewBackButtonVisibility.Disabled:
            case AppViewBackButtonVisibility.Visible:
                if (IsTitleBarTextBlockForwardBegun)
                {
                    goto default;
                }
                TitleBarTextBlockForward.Begin();
                IsTitleBarTextBlockForwardBegun = true;
                break;
            case AppViewBackButtonVisibility.Collapsed:
                TitleBarTextBlockBack.Begin();
                IsTitleBarTextBlockForwardBegun = false;
                break;
            default:
                break;
        }
    }

    private void StartTitleBarAnimation(Visibility visibility)
    {
        if (IsFirstRun)
        {
            IsFirstRun = false;
            TitleBar.Visibility = visibility;
            return;
        }

        switch (visibility)
        {
            case Visibility.Visible:
                TitleBarShow.Begin();
                break;
            case Visibility.Collapsed:
            default:
                break;
        }
        TitleBar.Visibility = visibility;
    }
}
