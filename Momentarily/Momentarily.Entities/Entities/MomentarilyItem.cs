using Apeek.Entities.Entities;
using Momentarily.Common;
using Momentarily.Common.Definitions;
using System.Collections.Generic;
namespace Momentarily.Entities.Entities
{
    public class MomentarilyItem : Good
    {        
        public MomentarilyItem(): base()
        {
        }
        public virtual int TypeId
        {
            get
            {
                return GetPropertyIdValueByName(MomentarilyItemProperties.MomentarilyItemType);
            }
        }
        public virtual float Deposit
        {
            get
            {
                var dep = GetPropertyValueByName(MomentarilyItemProperties.MomentarilyItemDeposit);
                float d = 0;
                if (dep != null)
                {
                    float.TryParse(dep, out d);
                }
                return d;
            }
        }
        public virtual string Location
        {
            get { return GetPropertyValueByName(MomentarilyItemProperties.MomentarilyItemLocation); }
        }
        public virtual string PickUpLocation
        {
            get { return GetPropertyValueByName(MomentarilyItemProperties.MomentarilyItemPickUpLocation); }
        }
        public static MomentarilyItem CreateEmpty()
        {
            var item = new MomentarilyItem();            
            item.AddEmptyProperties();
            //item.GoodPropertyValues.Add(MomentarilyItemProperties.MomentarilyItemType, new GoodPropertyValue());
            //item.GoodPropertyValues.Add(MomentarilyItemProperties.MomentarilyItemLocation, new GoodPropertyValue());
            //item.GoodPropertyValues.Add(MomentarilyItemProperties.MomentarilyItemDeposit, new GoodPropertyValue());
            //item.GoodPropertyValues.Add(MomentarilyItemProperties.MomentarilyItemPickUpLocation, new GoodPropertyValue());
            return item;
        }
        public virtual void AddEmptyProperties()
        {
            if (!GoodPropertyValues.ContainsKey(MomentarilyItemProperties.MomentarilyItemType)) GoodPropertyValues.Add(MomentarilyItemProperties.MomentarilyItemType, new GoodPropertyValue());
            if (!GoodPropertyValues.ContainsKey(MomentarilyItemProperties.MomentarilyItemLocation)) GoodPropertyValues.Add(MomentarilyItemProperties.MomentarilyItemLocation, new GoodPropertyValue());
            if (!GoodPropertyValues.ContainsKey(MomentarilyItemProperties.MomentarilyItemDeposit)) GoodPropertyValues.Add(MomentarilyItemProperties.MomentarilyItemDeposit, new GoodPropertyValue());
            if (!GoodPropertyValues.ContainsKey(MomentarilyItemProperties.MomentarilyItemPickUpLocation)) GoodPropertyValues.Add(MomentarilyItemProperties.MomentarilyItemPickUpLocation, new GoodPropertyValue());
        }
    }
}
