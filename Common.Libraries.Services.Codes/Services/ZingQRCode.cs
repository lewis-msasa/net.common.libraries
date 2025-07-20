
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZXing;
using ZXing.QrCode;

namespace Common.Libraries.Services.Services
{
    /*public class ZingQRCode : IQRCodeService
    {
        public Task<byte[]> GenerateBarCode(string content, string filePath)
        {
            QrCodeEncodingOptions options = new()
            {
                DisableECI = true,
                CharacterSet = "UTF-8",
                Width = 500,
                Height = 500
            };

            var writer = new BarcodeWriter<Bitmap>()
            {
                Format = BarcodeFormat.CODE_128,
                Options = options
            };
            Bitmap qrCodeBitmap = writer.Write(content);
            qrCodeBitmap.Save(filePath);
            return qrCodeBitmap;
        }

        public Task<byte[]> GenerateQRCode(string content, string filePath)
        {
            QrCodeEncodingOptions options = new()
            {
                DisableECI = true,
                CharacterSet = "UTF-8",
                Width = 500,
                Height = 500
            };

            var writer = new BarcodeWriter<Bitmap>()
            {
                Format = BarcodeFormat.QR_CODE,
                Options = options
            };
            Bitmap qrCodeBitmap = writer.Write(content);
            qrCodeBitmap.Save(filePath);
        }
    }*/
}
