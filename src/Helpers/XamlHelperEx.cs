using WUXCNavigationViewBackButtonVisible = Windows.UI.Xaml.Controls.NavigationViewBackButtonVisible;
using MUXCNavigationViewBackButtonVisible = Microsoft.UI.Xaml.Controls.NavigationViewBackButtonVisible;

namespace TerraHistoricus.Uwp.Helpers;

public static class XamlHelperEx
{
    public static WUXCNavigationViewBackButtonVisible ToNavigationViewBackButtonVisibility(bool value)
    {
        return value switch
        {
            true => WUXCNavigationViewBackButtonVisible.Visible,
            false => WUXCNavigationViewBackButtonVisible.Collapsed,
        };
    }
    
    public static WUXCNavigationViewBackButtonVisible ToNavigationViewBackButtonVisibility(bool? value)
    {
        return value switch
        {
            true => WUXCNavigationViewBackButtonVisible.Visible,
            false => WUXCNavigationViewBackButtonVisible.Collapsed,
            _ => WUXCNavigationViewBackButtonVisible.Auto
        };
    }
    
    public static WUXCNavigationViewBackButtonVisible ReverseNavigationViewBackButtonVisibility(bool value)
    {
        return value switch
        {
            false => WUXCNavigationViewBackButtonVisible.Visible,
            true => WUXCNavigationViewBackButtonVisible.Collapsed
        };
    }
    
    public static WUXCNavigationViewBackButtonVisible ReverseNavigationViewBackButtonVisibility(bool? value)
    {
        return value switch
        {
            false => WUXCNavigationViewBackButtonVisible.Visible,
            true => WUXCNavigationViewBackButtonVisible.Collapsed,
            _ => WUXCNavigationViewBackButtonVisible.Auto
        };
    }

    public static MUXCNavigationViewBackButtonVisible ToMUXCNavigationViewBackButtonVisibility(bool value)
    {
        return value switch
        {
            true => MUXCNavigationViewBackButtonVisible.Visible,
            false => MUXCNavigationViewBackButtonVisible.Collapsed,
        };
    }
    
    public static MUXCNavigationViewBackButtonVisible ToMUXCNavigationViewBackButtonVisibility(bool? value)
    {
        return value switch
        {
            true => MUXCNavigationViewBackButtonVisible.Visible,
            false => MUXCNavigationViewBackButtonVisible.Collapsed,
            _ => MUXCNavigationViewBackButtonVisible.Auto
        };
    }
    
    public static MUXCNavigationViewBackButtonVisible ReverseMUXCNavigationViewBackButtonVisibility(bool value)
    {
        return value switch
        {
            false => MUXCNavigationViewBackButtonVisible.Visible,
            true => MUXCNavigationViewBackButtonVisible.Collapsed
        };
    }
    
    public static MUXCNavigationViewBackButtonVisible ReverseMUXCNavigationViewBackButtonVisibility(bool? value)
    {
        return value switch
        {
            false => MUXCNavigationViewBackButtonVisible.Visible,
            true => MUXCNavigationViewBackButtonVisible.Collapsed,
            _ => MUXCNavigationViewBackButtonVisible.Auto
        };
    }
}
