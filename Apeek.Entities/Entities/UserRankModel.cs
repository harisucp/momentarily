using System;
using System.Collections.Generic;
namespace Apeek.Entities.Entities
{
    public class UserRankModel
    {
        public int PersonId { get; set; }
        [RankField]
        public string Email { get; set; }
        [RankFieldDescription]
        public string Description { get; set; }
        [RankFieldList]
        public List<ServiceRankModel> ServiceRankModels { get; set; }
        [RankField]
        public string AddressLine1 { get; set; }
        [RankField]
        public string UserImagesCount { get; set; }
        [RankFieldList]
        public List<PhoneNumberRankModel> PhoneNumbers { get; set; }
        public UserRankModel()
        {
            ServiceRankModels = new List<ServiceRankModel>();
            PhoneNumbers = new List<PhoneNumberRankModel>();
        }
    }
    [AttributeUsage(AttributeTargets.Property)]
    public class RankField : Attribute{ }
    [AttributeUsage(AttributeTargets.Property)]
    public class RankFieldDescription : Attribute{ }
    [AttributeUsage(AttributeTargets.Property)]
    public class RankFieldList : Attribute { }
}