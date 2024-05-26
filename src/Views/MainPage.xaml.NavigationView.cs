namespace TerraHistoricus.Uwp.Views;

public partial class MainPage
{
    private void OnNavigationViewItemInvoked(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewItemInvokedEventArgs args)
    {
        string tag = args.InvokedItemContainer.Tag as string;
        if (args.IsSettingsInvoked && ContentFrame.CurrentSourcePageType != typeof(SettingsPage))
        {
            ContentFrameNavigationHelper.Navigate(typeof(SettingsPage), transitionInfo: CommonValues.DefaultTransitionInfo);
        }
        else
        {
            NavigateByNavViewItemTag(tag);
        }
    }

    private void OnNavigationViewLoaded(object sender, RoutedEventArgs e)
    {
        if (sender is Microsoft.UI.Xaml.Controls.NavigationView view && view.SettingsItem is Microsoft.UI.Xaml.Controls.NavigationViewItemBase settings)
        {
            settings.AccessKeyInvoked += OnNavigationViewItemAccessKeyInvoked;
            settings.AccessKey = "T";
        }
    }

    private void OnNavigationViewItemAccessKeyInvoked(UIElement sender, AccessKeyInvokedEventArgs args)
    {
        if (sender == NavigationView.SettingsItem && ContentFrame.CurrentSourcePageType != typeof(SettingsPage))
        {
            ContentFrameNavigationHelper.Navigate(typeof(SettingsPage), transitionInfo: CommonValues.DefaultTransitionInfo);
        }
        else
        {
            var item = sender as Microsoft.UI.Xaml.Controls.NavigationViewItemBase;

            string tag = item.Tag as string;
            NavigateByNavViewItemTag(tag);
        }
    }

    private void OnNavigationViewBackRequested(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewBackRequestedEventArgs args)
    {
        if (ContentFrameNavigationHelper.CanGoBack)
        {
            ContentFrameNavigationHelper.GoBack();
        }
    }

    private void NavigateByNavViewItemTag(string tag)
    {
        if (tag == "ComicList" && ContentFrame.CurrentSourcePageType != typeof(ComicListPage))
        {
            ContentFrameNavigationHelper.Navigate(typeof(ComicListPage), transitionInfo: CommonValues.DefaultTransitionInfo);
        }
        else if (tag == "RecommendPage" && ContentFrame.CurrentSourcePageType != typeof(RecommendPage))
        {
            ContentFrameNavigationHelper.Navigate(typeof(RecommendPage), transitionInfo: CommonValues.DefaultTransitionInfo);
        }
        //else if (tag == "DownloadPage" && ContentFrame.CurrentSourcePageType != typeof(DownloadPage))
        //{
        //    ContentFrameNavigationHelper.Navigate(typeof(DownloadPage), transitionInfo: CommonValues.DefaultTransitionInfo);
        //}
    }

    /// <summary>
    /// 改变导航视图的选择项
    /// </summary>
    private void ChangeSelectedItemOfNavigationView()
    {
        Type currentSourcePageType = ContentFrame.SourcePageType;

        if (currentSourcePageType == typeof(ComicListPage))
        {
            NavigationView.SelectedItem = ComicListItem;
        }
        else if (currentSourcePageType == typeof(RecommendPage))
        {
            NavigationView.SelectedItem = RecommendPageItem;
        }
        else if (currentSourcePageType == typeof(ComicDetailPage))
        {
            NavigationView.SelectedItem = null;
        }
        else if (currentSourcePageType == typeof(SettingsPage))
        {
            NavigationView.SelectedItem = NavigationView.SettingsItem;
        }
#if DEBUG
        else
        {
            System.Diagnostics.Debugger.Break();
        }
#endif
    }
}
