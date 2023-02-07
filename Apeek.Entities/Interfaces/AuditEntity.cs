using System;
using Apeek.Entities.TypeConverters;
namespace Apeek.Entities.Interfaces
{
    public interface IAuditEntity
    {
        DateTime CreateDate { get; set; }
        DateTime ModDate { get; set; }
        int ModBy { get; set; }
        int CreateBy { get; set; }
    }
    public abstract class AuditEntity : IEntity, IAuditEntity
    {
        public abstract int Id { get; set; }
        public abstract string TableName { get; }
        public virtual DateTime CreateDate { get; set; }
        public virtual DateTime ModDate { get; set; }
        public virtual int ModBy { get; set; }
        public virtual int CreateBy { get; set; }
        public virtual void UpdateAuditData(int userId)
        {
            var dateTime = DateTime.Now;
            if (Id == 0 || CreateDate == DateTime.MinValue)
            {
                CreateDate = dateTime;
                CreateBy = userId;
            }
            ModDate = dateTime;
            ModBy = userId;
        }
    }
    public abstract class AuditEntityUt : AuditEntity
    {
        public virtual double CreateDateUt { get; set; }
        public override DateTime CreateDate { get { return DateTimeConverter.ConvertFromUnixTimestamp(CreateDateUt); } set { CreateDateUt = DateTimeConverter.ConvertToUnixTimestamp(value); } }
        public virtual double ModDateUt { get; set; }
        public override DateTime ModDate { get { return DateTimeConverter.ConvertFromUnixTimestamp(ModDateUt); } set { ModDateUt = DateTimeConverter.ConvertToUnixTimestamp(value); } }
        public override void UpdateAuditData(int userId)
        {
            var dateTime = DateTimeConverter.ConvertToUnixTimestamp(DateTime.Now);
            if (Id == 0)
            {
                CreateDateUt = dateTime;
                CreateBy = userId;
            }
            ModDateUt = dateTime;
            ModBy = userId;
        }
    }
}