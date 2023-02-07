using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Apeek.Common;
using Apeek.Common.Logger;
using ExifLib;
using ImageResizer;
namespace Apeek.Core.Services.Impl.Img
{
    public class RotateImage : ImgProcessorHandler
    {
        public override void HandleImage(Common.Models.ImgProcessorHandlerTarget imageHandlerTarget)
        {
            var mStream = new MemoryStream();
            var result = ExifDataRotateImage(imageHandlerTarget.Stream, mStream);
            if (result)
            {
                imageHandlerTarget.Stream = mStream;
            }
            if (_nextImgProcessorHandler != null)
                _nextImgProcessorHandler.HandleImage(imageHandlerTarget);
            mStream.Close();
        }
        private bool ExifDataRotateImage(Stream fileMemoryStream, Stream outMemoryStream)
        {
            try
            {
                using (var bmpImage = new Bitmap(fileMemoryStream))
                {
                    fileMemoryStream.Seek(0, SeekOrigin.Begin);
                    using (var exifReader = new ExifReader(fileMemoryStream))
                    {
                        ushort orientation;
                        if (exifReader.GetTagValue(ExifTags.Orientation, out orientation))
                        {
                            var flip = OrientationToFlipType(orientation.ToString());
                            if (flip != RotateFlipType.RotateNoneFlipNone)
                            {
                               // bmpImage.RotateFlip(flip);
                            }
                        }
                        bmpImage.Save(outMemoryStream, ImageFormat.Jpeg);
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Ioc.Get<IDbLogger>().LogWarning(LogSource.ImageProcessor, string.Format("Rotate image fail. Ex: {0}.", ex));
            }
            return false;
        }
        private static RotateFlipType OrientationToFlipType(string orientation)
        {
            switch (int.Parse(orientation))
            {
                case 1:
                    return RotateFlipType.RotateNoneFlipNone;
                case 2:
                    return RotateFlipType.RotateNoneFlipX;
                case 3:
                    return RotateFlipType.Rotate180FlipNone;
                case 4:
                    return RotateFlipType.Rotate180FlipX;
                case 5:
                    return RotateFlipType.Rotate90FlipX;
                case 6:
                    return RotateFlipType.Rotate90FlipNone;
                case 7:
                    return RotateFlipType.Rotate270FlipX;
                case 8:
                    return RotateFlipType.Rotate270FlipNone;
                default:
                    return RotateFlipType.RotateNoneFlipNone;
            }
        }
    }
}