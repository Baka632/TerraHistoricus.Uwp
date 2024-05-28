using System.Runtime.InteropServices;
using System.Text.Json;
using Microsoft.Toolkit.Uwp.Helpers;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.Web.Http;

namespace TerraHistoricus.Uwp.Helpers;

/// <summary>
/// 为应用程序数据提供文件中缓存的类
/// </summary>
internal static class FileCacheHelper
{
    private const string DefaultComicSmallCoverCacheFolderName = "ComicSmallCover";
    private const string DefaultComicLargeCoverCacheFolderName = "ComicLargeCover";
    private const string DefaultRecommendEpisodeCoverCacheFolderName = "RecommendEpisodeCover";
    private static readonly StorageFolder tempFolder = ApplicationData.Current.TemporaryFolder;

    /// <summary>
    /// 使用指定的 <see cref="ComicDetail"/> 实例，在漫画封面缓存文件夹创建漫画封面文件
    /// </summary>
    /// <param name="comicDetail">一个 <see cref="ComicDetail"/> 实例</param>
    public static async Task StoreComicCoverAsync(ComicDetail comicDetail)
    {
        await StoreCoverByUriAndCid(comicDetail.CoverUri, comicDetail.Cid, DefaultComicLargeCoverCacheFolderName);
    }

    /// <summary>
    /// 使用指定的 <see cref="ComicInfo"/> 实例，在漫画封面缓存文件夹创建漫画封面文件
    /// </summary>
    /// <param name="comicInfo">一个 <see cref="ComicInfo"/> 实例</param>
    public static async Task StoreComicCoverAsync(ComicInfo comicInfo)
    {
        await StoreCoverByUriAndCid(comicInfo.CoverUri, comicInfo.Cid, DefaultComicSmallCoverCacheFolderName);
    }

    /// <summary>
    /// 使用指定的 Uri 字符串与 CID 字符串，在漫画封面缓存文件夹创建漫画封面文件
    /// </summary>
    /// <param name="uri">漫画封面的 Uri</param>
    /// <param name="cid">漫画的 CID</param>
    /// <param name="folderName">漫画封面缓存文件夹名称</param>
    private static async Task StoreCoverByUriAndCid(string uri, string cid, string folderName)
    {
        Uri coverUri = new(uri, UriKind.Absolute);

        try
        {
            using HttpClient httpClient = new();
            using HttpResponseMessage result = await httpClient.GetAsync(coverUri);

            using InMemoryRandomAccessStream stream = new();
            await result.Content.WriteToStreamAsync(stream);
            stream.Seek(0);
            string fileName = $"{cid}.jpg";
            await StoreComicCoverAsync(fileName, stream, folderName);
        }
        catch (COMException)
        {
            return;
        }
    }

    /// <summary>
    /// 使用指定的文件名与随机访问流，在漫画封面缓存文件夹创建文件
    /// </summary>
    /// <param name="fileName">文件名</param>
    /// <param name="stream">漫画封面的随机访问流</param>
    /// <param name="folderName">漫画封面缓存文件夹名称</param>
    private static async Task StoreComicCoverAsync(string fileName, IRandomAccessStream stream, string folderName)
    {
        StorageFolder coverFolder = await tempFolder.CreateFolderAsync(folderName, CreationCollisionOption.OpenIfExists);

        StorageFile file = await coverFolder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
        using StorageStreamTransaction transaction = await file.OpenTransactedWriteAsync();
        await RandomAccessStream.CopyAsync(stream, transaction.Stream);
        await transaction.CommitAsync();
    }

    /// <summary>
    /// 通过指定的 <see cref="ComicDetail"/> 实例获取漫画封面的随机访问流
    /// </summary>
    /// <param name="comicDetail"><see cref="ComicDetail"/> 的实例</param>
    /// <returns>包含漫画封面数据的 <see cref="IRandomAccessStream"/></returns>
    public static async Task<IRandomAccessStream> GetComicCoverStreamAsync(ComicDetail comicDetail)
    {
        string fileName = $"{comicDetail.Cid}.jpg";
        return await GetComicCoverStreamAsync(fileName, DefaultComicLargeCoverCacheFolderName);
    }

    /// <summary>
    /// 通过指定的 <see cref="ComicInfo"/> 实例获取漫画封面的随机访问流
    /// </summary>
    /// <param name="comicInfo"><see cref="ComicInfo"/> 的实例</param>
    /// <returns>包含漫画封面数据的 <see cref="IRandomAccessStream"/></returns>
    public static async Task<IRandomAccessStream> GetComicCoverStreamAsync(ComicInfo comicInfo)
    {
        string fileName = $"{comicInfo.Cid}.jpg";
        return await GetComicCoverStreamAsync(fileName, DefaultComicSmallCoverCacheFolderName);
    }

    /// <summary>
    /// 通过指定的文件名获取漫画封面的随机访问流
    /// </summary>
    /// <param name="fileName">漫画封面的文件名</param>
    /// <param name="folderName">漫画封面缓存文件夹名称</param>
    /// <returns>包含漫画封面数据的 <see cref="IRandomAccessStream"/></returns>
    private static async Task<IRandomAccessStream> GetComicCoverStreamAsync(string fileName, string folderName)
    {
        StorageFolder coverFolder = await tempFolder.CreateFolderAsync(folderName, CreationCollisionOption.OpenIfExists);

        if (coverFolder != null)
        {
            StorageFile file = await coverFolder.GetFileAsync(fileName);
            return await file?.OpenReadAsync();
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// 通过指定的 <see cref="ComicDetail"/> 实例获取指向漫画封面的 Uri
    /// </summary>
    /// <param name="comicDetail"><see cref="ComicDetail"/> 的实例</param>
    /// <returns>指向漫画封面的 <see cref="Uri"/></returns>
    public static async Task<Uri> GetComicCoverUriAsync(ComicDetail comicDetail)
    {
        string fileName = $"{comicDetail.Cid}.jpg";
        return await GetComicCoverUriAsync(fileName, DefaultComicLargeCoverCacheFolderName);
    }

    /// <summary>
    /// 通过指定的 <see cref="ComicInfo"/> 实例获取指向漫画封面的 Uri
    /// </summary>
    /// <param name="comicInfo"><see cref="ComicInfo"/> 的实例</param>
    /// <returns>指向漫画封面的 <see cref="Uri"/></returns>
    public static async Task<Uri> GetComicCoverUriAsync(ComicInfo comicInfo)
    {
        string fileName = $"{comicInfo.Cid}.jpg";
        return await GetComicCoverUriAsync(fileName, DefaultComicSmallCoverCacheFolderName);
    }

    /// <summary>
    /// 通过指定的 <see cref="ComicInfo"/> 实例获取指向漫画封面的 Uri
    /// </summary>
    /// <param name="info"><see cref="ComicInfo"/> 的实例</param>
    /// <returns>指向漫画封面的 <see cref="Uri"/></returns>
    public static async Task<Uri> GetComicCoverUriAsync(RecommendComicInfo info)
    {
        string fileName = $"{info.EpisodeCid}.jpg";
        return await GetComicCoverUriAsync(fileName, DefaultRecommendEpisodeCoverCacheFolderName);
    }

    /// <summary>
    /// 通过指定的文件名获取指向漫画封面的 Uri
    /// </summary>
    /// <param name="fileName">漫画封面的文件名</param>
    /// <param name="folderName">漫画封面缓存文件夹名称</param>
    /// <returns>指向漫画封面的 <see cref="Uri"/></returns>
    private static async Task<Uri> GetComicCoverUriAsync(string fileName, string folderName)
    {
        StorageFolder coverFolder = await tempFolder.CreateFolderAsync(folderName, CreationCollisionOption.OpenIfExists);

        if (coverFolder != null && await coverFolder.FileExistsAsync(fileName))
        {
            return new Uri($"ms-appdata:///temp/{folderName}/{fileName}", UriKind.Absolute);
        }
        else
        {
            return null;
        }
    }
}
