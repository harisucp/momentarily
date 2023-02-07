using Apeek.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apeek.Entities.Entities
{
   public class GoodImgNew : AuditEntity
    {


        public static string _TableName = "c_good_img";
        public override string TableName { get { return _TableName; } }
        public override int Id { get; set; }
        public virtual int? GoodId { get; set; }
        public virtual int UserId { get; set; }
        public virtual int Type { get; set; }
        public virtual int Sequence { get; set; }
        public virtual string FileName { get; set; }
        public virtual string Folder { get; set; }
    }
}
