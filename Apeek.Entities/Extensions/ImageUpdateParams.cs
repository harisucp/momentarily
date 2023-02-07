using System;
using System.Runtime.Serialization;
using System.Text;
using Apeek.Entities.Constants;
namespace Apeek.Entities.Extensions
{
    [Serializable]
    [DataContract]
    public class ImageUpdateParams
    {
        [DataMember]
        public string srotate { get; set; }
        [DataMember]
        public string width { get; set; }
        [DataMember]
        public string height { get; set; }
        [DataMember]
        public string maxwidth { get; set; }
        [DataMember]
        public string maxheight { get; set; }
        [DataMember]
        public string format { get; set; }
        [DataMember]
        public string bgcolor { get; set; }
        [DataMember]
        public string mode { get; set; }
        [DataMember]
        public string scale { get; set; }
        public const char _delim = '&';
        public ImageUpdateParams()
        {
            format = ConstantsImage.DefaultImageExt;
        }
        public override string ToString()
        {
            var sb = new StringBuilder();
            AppendParam("srotate", srotate, sb);
            AppendParam("width", width, sb);
            AppendParam("height", height, sb);
            AppendParam("maxwidth", maxwidth, sb);
            AppendParam("maxheight", maxheight, sb);
            AppendParam("format", format, sb);
            AppendParam("bgcolor", bgcolor, sb);
            AppendParam("mode", mode, sb);
            AppendParam("scale", scale, sb);
            return sb.ToString().TrimEnd(new []{_delim});
        }
        private void AppendParam(string paramName, string value, StringBuilder sb)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                sb.Append(string.Format("{0}={1}", paramName, value));
                sb.Append(_delim);
            }
        }
        public ImageUpdateParams MergeAndCreateNew(ImageUpdateParams imageSize)
        {
            var iup = (ImageUpdateParams)this.MemberwiseClone();
            if (string.IsNullOrWhiteSpace(iup.srotate))
                iup.srotate = imageSize.srotate;
            if (string.IsNullOrWhiteSpace(iup.width))
                iup.width = imageSize.width;
            if (string.IsNullOrWhiteSpace(iup.height))
                iup.height = imageSize.height;
            if (string.IsNullOrWhiteSpace(iup.maxwidth))
                iup.maxwidth = imageSize.maxwidth;
            if (string.IsNullOrWhiteSpace(iup.maxheight))
                iup.maxheight = imageSize.maxheight;
            if (string.IsNullOrWhiteSpace(iup.format))
                iup.format = imageSize.format;
            if (string.IsNullOrWhiteSpace(iup.bgcolor))
                iup.bgcolor = imageSize.bgcolor;
            if (string.IsNullOrWhiteSpace(iup.mode))
                iup.mode = imageSize.mode;
            if (string.IsNullOrWhiteSpace(iup.scale))
                iup.scale = imageSize.scale;
            return iup;
        }
    }
}