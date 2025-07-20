
using QRCoder;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Libraries.Services.Services
{
    public class ORCodeService : IQRCodeService
    {
        public Task<byte[]> GenerateBarCode(string content, string filePath)
        {
            throw new NotImplementedException();
        }

        public async Task<byte[]> GenerateQRCode(string content, string filePath)
        {
            QRCodeGenerator QRGen = new QRCodeGenerator();
            QRCodeData Qrinfo = QRGen.CreateQrCode(content, QRCodeGenerator.ECCLevel.Q);
            var qRCoder = new PngByteQRCode(Qrinfo);

            var bitmapArray =  await Task.Run(() => qRCoder.GetGraphic(50));
            using (MemoryStream mStream = new MemoryStream(bitmapArray))
            {
                  var image = SixLabors.ImageSharp.Image.Load(mStream);
                  image.Save(filePath);
            }
            // Color 
            //Bitmap QRbitmap = qRCoder.GetGraphic(50, Color.Blue, Color.Gray, true);
            return bitmapArray ?? Array.Empty<byte>();
        }
    }
}
