using System.Collections.Generic;
using System.Text;
namespace Apeek.Entities.Entities
{
    public class PersonService
    {
        public static string TableName = "c_person_service";
        public virtual User User { get; set; }
        public virtual List<GoodImg> Images { get; set; }
        public virtual Service Service { get; set; }
        public virtual ServiceLang ServiceLang { get; set; }
        public virtual string Description { get; set; }
        public virtual string Header { get; set; }
        public virtual double? Price { get; set; }
        public virtual double? FromPrice { get; set; }
        public virtual double? ToPrice { get; set; }
        public virtual string RawPrice { get; set; }
        public virtual string Metric { get; set; }
        public PersonService()
        {
            Images = new List<GoodImg>();
        }
        public override string ToString()
        {
            var strBuilder = new StringBuilder();
            if (User != null)
                strBuilder.Append(string.Format("PersonId: {0}; ", User.Id));
            if (Service != null)
                strBuilder.Append(string.Format("ServiceId: {0}; ", Service.Id));
            strBuilder.Append(string.Format("RawPrice: {0}; ", RawPrice));
            if (Price.HasValue)
                strBuilder.Append(string.Format("Price: {0}; ", Price.Value));
            if (FromPrice.HasValue)
                strBuilder.Append(string.Format("FromPrice: {0}; ", FromPrice.Value));
            if (ToPrice.HasValue)
                strBuilder.Append(string.Format("ToPrice: {0}; ", ToPrice.Value));
            strBuilder.Append(string.Format("Metric: {0}; ", Metric));
            strBuilder.Append(string.Format("Description: {0}; ", Description));
            strBuilder.Append(string.Format("Header: {0}; ", Header));
            return strBuilder.ToString();
        }
        public override bool Equals(object obj)
        {
            var other = obj as PersonService;
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return this.User.Id == other.User.Id &&
                this.Service.Id == other.Service.Id;
        }
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = GetType().GetHashCode();
                hash = (hash * 31) ^ this.User.Id.GetHashCode();
                hash = (hash * 31) ^ this.Service.Id.GetHashCode();
                return hash;
            }
        }
    }
}