using Apeek.Entities.Entities;
using Apeek.ViewModels.Models;
namespace Apeek.ViewModels.Mappers.Imp
{
    public class GoodRequestMapper : IGoodRequestMapper
    {
        public GoodRequestViewModel Map(GoodRequest source, GoodRequestViewModel dest)
        {
            dest.Id = source.Id;
            dest.UserId = source.UserId;
            dest.GoodId = source.GoodId;
            dest.UserName = source.User.FirstName;
            dest.GoodName = source.Good.Name;
            dest.GoodDescription = source.Good.Description;
            dest.StatusId = source.StatusId;
            dest.StatusName = source.RequestStatus.StatusName;
            dest.StartDate = source.GoodBooking.StartDate;
            dest.ShippingAddress = source.GoodBooking.ShippingAddress;
            dest.EndDate = source.GoodBooking.EndDate;
            dest.CreateDate = source.CreateDate;
            dest.DiliveryPrice = source.DiliveryPrice;            
            dest.CustomerServiceFee = source.CustomerServiceFee;
            dest.CustomerCharity = source.CustomerCharity;
            dest.Price = source.Price;
            dest.Days = source.Days;
            dest.DaysCost = source.DaysCost;
            dest.CustomerCost = source.CustomerCost;
            dest.CustomerServiceFee = source.CustomerServiceFee;
            dest.CustomerServiceFeeCost = source.CustomerServiceFeeCost;
            dest.CustomerCharity = source.CustomerCharity;
            dest.CustomerCharityCost = source.CustomerCharityCost;
            dest.SharerCost = source.SharerCost;
            dest.SharerServiceFee = source.SharerServiceFee;
            dest.SharerServiceFeeCost = source.SharerServiceFeeCost;
            dest.SharerCharity = source.SharerCharity;
            dest.SharerCharityCost = source.SharerCharityCost;
            dest.DiliveryCost = source.DiliveryCost;
            dest.ShippingDistance = source.ShippingDistance;
            dest.DiliveryPrice = source.DiliveryPrice;
            dest.SecurityDeposit = source.SecurityDeposit;
            if (source.User != null)
            {
                dest.UserEmail = source.User.Email;
                dest.UserName = source.User.FirstName;
            }
            return dest;
        }
    }
}
