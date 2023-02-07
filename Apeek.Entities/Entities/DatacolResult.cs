using System;
namespace Apeek.Entities.Entities
{
    public class DatacolResult
    {
        public virtual int Id { get; set; }
        public virtual string ServiceName { get; set; }
        public virtual string RubricName { get; set; }
        public virtual string CityName { get; set; }
        public virtual string AddressLine { get; set; }
        public virtual string Description { get; set; }
        public virtual string UserName { get; set; }
        public virtual string Price { get; set; }
        public virtual string PhoneNums { get; set; }
        public virtual string SourceId { get; set; }
        public virtual string SourceUrl { get; set; }
        public virtual string SourceName { get; set; }
        public virtual string SourceCreateDate { get; set; }
        public virtual int ProcessStatus { get; set; }
        public virtual string ErrorReason { get; set; }
        public virtual int LangId { get; set; }
        public virtual DateTime? CreateDate { get; set; }
        public virtual int? ServiceParentIdForNewServices { get; set; }
        public DatacolResult()
        {
            LangId = 2;
        }
    }
}