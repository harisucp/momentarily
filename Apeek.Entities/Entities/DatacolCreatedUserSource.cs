namespace Apeek.Entities.Entities
{
    public class DatacolCreatedUserSource
    {
        public virtual int UserId { get; set; }
        public virtual int DatacolResultId { get; set; }
        public override bool Equals(object obj)
        {
            var other = obj as DatacolCreatedUserSource;
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return this.UserId == other.UserId &&
                this.DatacolResultId == other.DatacolResultId;
        }
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = GetType().GetHashCode();
                hash = (hash * 31) ^ this.UserId.GetHashCode();
                hash = (hash * 31) ^ this.DatacolResultId.GetHashCode();
                return hash;
            }
        }
    }
}