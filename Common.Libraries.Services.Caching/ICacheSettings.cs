
namespace Common.Libraries.Services.Caching
{
    public interface ICacheSettings
    {
        int DefaultAbsoluteExpireTime { get; set; }
        int DefaultSlidingExpireTime { get; set; }
    }
}