using System;
using System.Collections.Generic;
using System.IO;
using Apeek.Common.Interfaces;
using Apeek.Entities.Constants;
using Apeek.Entities.Entities;
using Apeek.Entities.Extensions;
namespace Apeek.Common.Models
{
    [Serializable]
    public class ImgProcessorHandlerTarget : IMsmqMessage
    {
        public byte[] Bytes { get; set; }
        public string FileName { get; set; }
        public int UserId { get; set; }
        public int Type { get; set; }
        public int Sequence { get; set; }
        public Stream Stream { get; set; }
        public int? GoodId { get; set; }
        public ImageFolder ImageFolder { get; set; }
        public OperationType OperationType { get; set; }
        public ImageUpdateParams UpdateParams { get; set; }
        public List<IUserImg> Images { get; set; }
        public Dictionary<ImageType, ImageUpdateParams> DefaultImageSizes { get; set; }
        public ImgProcessorHandlerTarget()
        {
            UpdateParams = new ImageUpdateParams();
        }
        public override string ToString()
        {
            return string.Format(@"UserId: {0};\n FileName: {1};\n Sequence: {2};\n 
Type: {3};\n GoodId: {4};\n OperationType: {5};\n
UpdateParams: {6}",
                  UserId, FileName, Sequence, Type, GoodId, OperationType, UpdateParams == null ? string.Empty : UpdateParams.ToString());
        }
    }
}