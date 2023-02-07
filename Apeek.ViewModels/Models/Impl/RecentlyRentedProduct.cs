using Apeek.Common.Extensions;
using Apeek.Entities.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apeek.ViewModels.Models.Impl
{
   public class RecentlyRentedProduct
    {
        public int GoodId { get; set; }
        public int GoodRequestId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public double PricePerWeek { get; set; }
        public double PricePerMonth { get; set; }
        public bool RentPeriodDay { get; set; }
        public bool RentPeriodWeek { get; set; }
        public bool RentPeriodMonth { get; set; }
        public bool AgreeToDeliver { get; set; }
        public bool AgreeToShareImmediately { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Location { get; set; }
        public string Type { get; set; }
        public double Deposit { get; set; }
        public double CouponDiscount { get; set; }



        public string GoodImageFileName { get; set; }
        public string PickUpLoaction { get; set; }
        public string GoodImageUrl
        {
            get
            {
                return GoodImageFileName.ImageUrl(ImageFolder.Good.ToString());
            }
            set
            {
                GoodImageFileName = value;
            }
        }
        public int Count { get; set; }



    }
}
