using Apeek.Entities.Interfaces;
namespace Apeek.ViewModels.Mappers.Imp
{
    public class AuditEntityMapper<Src, Dest> : IAuditEntityMapper<Src, Dest>
        where Src : IAuditEntity
        where Dest : IAuditEntity
    {
        //public static void Map<Src, Dest>(Src source, Dest dest) 
        //    where Src : IAuditEntity
        //    where Dest : IAuditEntity
        //{
        //    dest.CreateBy = source.CreateBy;
        //    dest.CreateDate = source.CreateDate;
        //    dest.ModBy = source.ModBy;
        //    dest.ModDate = source.ModDate;
        //}
        public Dest AuditMap(Src source, Dest dest)
        {
            dest.CreateBy = source.CreateBy;
            dest.CreateDate = source.CreateDate;
            dest.ModBy = source.ModBy;
            dest.ModDate = source.ModDate;
            return dest;
        }
        public Src AuditMap(Dest source, Src dest)
        {
            dest.CreateBy = source.CreateBy;
            dest.CreateDate = source.CreateDate;
            dest.ModBy = source.ModBy;
            dest.ModDate = source.ModDate;
            return dest;
        }
    }
}