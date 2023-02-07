using System.Runtime.Serialization;
using Apeek.Common.Models;
using Apeek.Entities.Extensions;
namespace Apeek.ViewModels.Models
{
    [DataContract]
    public class ImgProcessorModel
    {
        [DataMember]
        public int? GoodId { get; set; }
        [DataMember]
        public string FileName { get; set; }
        [DataMember]
        public int? Sequence { get; set; }
        [DataMember]
        public ImageUpdateParams UpdateParams { get; set; }
    }
}