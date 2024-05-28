using Windows.Foundation.Metadata;
using Windows.UI.Xaml.Media.Animation;

namespace TerraHistoricus.Uwp;

/// <summary>
/// 提供应用中常用的值的类
/// </summary>
internal static class CommonValues
{
    #region Message Token
    public const string NotifyWillUpdateMediaMessageToken = "Notify_WillUpdateMedia_MessageToken";
    public const string NotifyUpdateMediaFailMessageToken = "Notify_UpdateMediaFail_MessageToken";
    public const string NotifyAppBackgroundChangedMessageToken = "Notify_AppBackgroundChanged_MessageToken";
    #endregion

    #region Cache Key
    public const string ComicInfoCacheKey = "ComicInfo_CacheKey";
    public const string RecommendComicInfoCacheKey = "RecommendComicInfo_CacheKey";
    public const string EpisodeUpdateInfosCacheKey = "EpisodeUpdateInfos_CacheKey";
    #endregion

    #region Animation Key
    #endregion

    #region Settings Key
    #endregion

    #region Data Package Type
    #endregion

    #region Other Common Things
    public static readonly NavigationTransitionInfo DefaultTransitionInfo;
    #endregion

    static CommonValues()
    {
        if (ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 7))
        {
            DefaultTransitionInfo = new SlideNavigationTransitionInfo()
            {
                Effect = SlideNavigationTransitionEffect.FromRight
            };
        }
        else
        {
            DefaultTransitionInfo = new DrillInNavigationTransitionInfo();
        }
    }
}
