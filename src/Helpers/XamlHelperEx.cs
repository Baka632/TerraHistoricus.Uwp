using WUXCNavigationViewBackButtonVisible = Windows.UI.Xaml.Controls.NavigationViewBackButtonVisible;
using MUXCNavigationViewBackButtonVisible = Microsoft.UI.Xaml.Controls.NavigationViewBackButtonVisible;
using Windows.UI.Xaml.Media.Imaging;

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

    public static Uri ToUri(string value)
    {
        if (value is null)
        {
            return null;
        }

        return new Uri(value);
    }

    public static BitmapImage ToBitmapImage(string value)
    {
        return Uri.TryCreate(value, UriKind.Absolute, out Uri uri)
            ? new BitmapImage(uri)
            : null;
    }

    public static DateTimeOffset ToDateTimeOffset(long unixTimeSeconds)
    {
        return DateTimeOffset.FromUnixTimeSeconds(unixTimeSeconds);
    }

    public static string ToDateString(long unixTimeSeconds, string format)
    {
        return ToDateTimeOffset(unixTimeSeconds).ToString(format);
    }

    public static string ConcatStringWithSeperator(string value1, string value2, string separator)
    {
        return $"{value1}{separator}{value2}";
    }
}
