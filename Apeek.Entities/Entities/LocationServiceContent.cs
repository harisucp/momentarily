using System.ComponentModel.DataAnnotations;
namespace Apeek.Entities.Entities
{
    public class LocationServiceContent
    {
        [Required]
        public virtual int ServiceId { get; set; }
        [Required]
        public virtual int LocationId { get; set; }
        public virtual int LangId { get; set; }
        [StringLength(5000, ErrorMessage = "Description cannot be longer than 5000 characters.")]
        public virtual string Description { get; set; }
        [StringLength(200, ErrorMessage = "MetaTitle cannot be longer than 200 characters.")]
        public virtual string MetaTitle { get; set; }
        [StringLength(200, ErrorMessage = "MetaDescr cannot be longer than 200 characters.")]
        public virtual string MetaDescr { get; set; }
        [StringLength(200, ErrorMessage = "MetaKeys cannot be longer than 200 characters.")]
        public virtual string MetaKeys { get; set; }
        [StringLength(200, ErrorMessage = "HeaderText cannot be longer than 200 characters.")]
        public virtual string HeaderText { get; set; }
        public override bool Equals(object obj)
        {
            var other = obj as LocationServiceContent;
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return this.ServiceId == other.ServiceId &&
                this.LocationId == other.LocationId &&
                this.LangId == other.LangId;
        }
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = GetType().GetHashCode();
                hash = (hash * 31) ^ this.ServiceId.GetHashCode();
                hash = (hash * 31) ^ this.LocationId.GetHashCode();
                hash = (hash * 31) ^ this.LangId.GetHashCode();
                return hash;
            }
        }
    }
}