using Apeek.Entities.Entities;
namespace Apeek.ViewModels.Models
{
    public class ImgModel : IUserImg
    {
        public int UserId { get; set; }
        public virtual int Type { get; set; }
        public virtual int Sequence { get; set; }
        public virtual string FileName { get; set; }
        public virtual string ImgUrl { get; set; }
    }
}