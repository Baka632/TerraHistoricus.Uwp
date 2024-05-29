﻿// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace TerraHistoricus.Uwp.Views;

/// <summary>
/// 可用于自身或导航至 Frame 内部的空白页。
/// </summary>
public sealed partial class EpisodeReadPage : Page
{
    public EpisodeReadViewModel ViewModel { get; } = new EpisodeReadViewModel();

    public EpisodeReadPage()
    {
        this.InitializeComponent();
    }

    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);

        if (e.Parameter is ValueTuple<ComicDetail, EpisodeInfo> data)
        {
            await ViewModel.Initialize(data.Item1, data.Item2);
        }
    }
}