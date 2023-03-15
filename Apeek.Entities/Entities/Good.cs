using System.Collections.Generic;
using Apeek.Entities.Interfaces;
using System.ComponentModel.DataAnnotations;
namespace Apeek.Entities.Entities
{
    public class Good : AuditEntity
    {
        public Good()
        {
            GoodPropertyValues = new Dictionary<string, GoodPropertyValue>();
            GoodLocation = new GoodLocation();
            StartEndDate = new GoodStartEndDate();            
        }
        public static string _TableName = "c_good";
        #region AuditEntity implementation
        public override string TableName { get { return _TableName; } }
        public override int Id { get; set; }
        #endregion
        public virtual string Name { get; set; }
        [Required(ErrorMessage = "Description is required.")]
        public virtual string Description { get; set; }
        [Required(ErrorMessage = "Minimum Rent Period is required.")]
        public virtual int MinimumRentPeriod { get; set; }
        public virtual double Price { get; set; }
        public virtual double PricePerWeek { get; set; }
        public virtual double PricePerMonth { get; set; }
        public virtual bool RentPeriodDay { get; set; }
        public virtual bool RentPeriodWeek { get; set; }
        public virtual bool RentPeriodMonth { get; set; }
        public virtual bool AgreeToDeliver { get; set; }
        public virtual bool AgreeToShareImmediately { get; set; }
        public virtual bool IsArchive { get; set; }
        public virtual bool IsApproved { get; set; }
        public virtual int CategoryId { get; set; }
        public virtual string CategorList { get; set; }
        public virtual Dictionary<string, GoodPropertyValue> GoodPropertyValues { get; set; }
        public virtual IEnumerable<UserImageModel> Images { get; set; }
        public virtual GoodLocation GoodLocation { get; set; }
        public virtual GoodStartEndDate StartEndDate { get; set; }
        public virtual IEnumerable<GoodImg> GoodImages { get; set; }
        public virtual User GoodOwner { get; set; }
        public virtual List<GoodImg> GoodImageslist { get; set; }
        public virtual UserGood UserGood { get; set; }
        public virtual IEnumerable<long> GoodShareDates { get; set; }
        public virtual string StartTime { get; set; }        public virtual string EndTime { get; set; }

        protected string GetPropertyValueByName(string name)
        {
            if (GoodPropertyValues.ContainsKey(name))
            {
                return GoodPropertyValues[name].Value;
            }
            return null;
        }
        protected int GetPropertyIdValueByName(string name)
        {
            if (GoodPropertyValues.ContainsKey(name))
            {
                if (GoodPropertyValues[name].PropertyValueDefinitionId.HasValue)
                {
                    return GoodPropertyValues[name].PropertyValueDefinitionId.Value;
                }
            }
            return 0;
        }
        protected void SetPropertyValue(string name, string valueString = null, int? valueId = null)
        {
            if (!GoodPropertyValues.ContainsKey(name))
            {
                GoodPropertyValues.Add(name, new GoodPropertyValue() { GoodId = Id });
            }
            if (valueId != null)
            {
                GoodPropertyValues[name].PropertyValueDefinitionId = valueId.Value;
            }
            else
            {
                GoodPropertyValues[name].Value = valueString;
            }
        }
    }
}