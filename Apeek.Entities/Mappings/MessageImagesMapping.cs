using Apeek.Entities.Entities;
using Apeek.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apeek.Entities.Mappings
{
    public class MessageImagesMapping : AuditEntityMap<MessageImages>
    {
        public MessageImagesMapping()
        {
            Table("c_message_images");
            Id(x => x.Id, "id");
            Map(x => x.MessageDetailId, "message_detail_id");
            Map(x => x.ImagePath, "image_path");
        }
    }
}
