using Apeek.Entities.Entities;
using Apeek.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apeek.Entities.Mappings
{
   public class GoodImgNewMap : AuditEntityMap<GoodImgNew>
    {
        public GoodImgNewMap()
        {
            Table("c_good_img");
            Id(x => x.Id, "id");
            Map(x => x.GoodId, "good_id");
            Map(x => x.UserId, "user_id");
            Map(x => x.Type, "type");
            Map(x => x.Sequence, "sequence");
            Map(x => x.FileName, "file_name");
            Map(x => x.Folder, "folder");
        }
    }
}
