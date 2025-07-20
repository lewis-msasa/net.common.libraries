
namespace Common.Libraries.Services.Services
{
    public interface IQRCodeService
    {
        Task<byte[]> GenerateBarCode(string content, string filePath);
        Task<byte[]> GenerateQRCode(string content, string filePath);
    }
}