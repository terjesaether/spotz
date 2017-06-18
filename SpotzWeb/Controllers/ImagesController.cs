using SpotzWeb.Models;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace SpotzWeb.Controllers
{
    public class ImagesController : Controller
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();

        public ActionResult Index(Guid id, int? thumb)
        {
            var file = _db.Images.SingleOrDefault(img => img.ImageId == id);
            if (file == null)
            {
                return HttpNotFound();
            }
            var content = thumb.HasValue ? ResizeImage(file.Data, thumb.Value) : file.Data;

            var contentType = GetContentType(file.Filename);
            return File(content, contentType);
        }

        private static string GetContentType(string filename)
        {
            switch (Path.GetExtension(filename))
            {
                case ".jpg":
                    return "image/jpeg";
                case ".png":
                    return "image/png";
                default:
                    return "image/jpeg";
            }
        }

        public static byte[] ResizeImage(byte[] bytes, int size)
        {
            var image = System.Drawing.Image.FromStream(new MemoryStream(bytes));
            int height;
            int width;
            if (image.Width > image.Height)
            {
                var ratio = (double)image.Height / (double)image.Width;
                height = size;
                width = (int)((double)size / ratio);
            }
            else
            {
                var ratio = (double)image.Width / (double)image.Height;
                width = size;
                height = (int)((double)size / ratio);
            }

            var bitmap = ResizeImage(image, width, height);
            var stream = new MemoryStream();
            bitmap.Save(stream, ImageFormat.Jpeg);

            return stream.ToArray();
        }
        public static Bitmap ResizeImage(System.Drawing.Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }
    }
}
