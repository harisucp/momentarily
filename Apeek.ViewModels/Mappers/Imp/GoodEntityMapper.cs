using System;
using System.Linq;
using Apeek.Common.Extensions;
using Apeek.Entities.Constants;
using Apeek.Entities.Entities;
using Apeek.ViewModels.Models;
namespace Apeek.ViewModels.Mappers.Imp
{
    public class GoodMapper : IGoodMapper
    {
        public IGoodViewModel Map(Good source, IGoodViewModel dest)
        {
            dest.GoodId = source.Id;
            dest.Name = source.Name;
            dest.Description = source.Description;
            dest.Price = source.Price;
            dest.PricePerWeek = source.PricePerWeek;
            dest.PricePerMonth = source.PricePerMonth;
            dest.RentPeriodDay = source.RentPeriodDay;
            dest.RentPeriodWeek = source.RentPeriodWeek;
            dest.RentPeriodMonth = source.RentPeriodMonth;
            dest.AgreeToDeliver = source.AgreeToDeliver;
            dest.AgreeToShareImmediately = source.AgreeToShareImmediately;
            //dest.Location = source..Location;
            dest.Latitude = source.GoodLocation.Latitude;
            dest.Longitude = source.GoodLocation.Longitude;
            return dest;
        }
        public Models.Impl.UserProfileGoodViewModel Map(Good source, Models.Impl.UserProfileGoodViewModel dest)
        {
            dest.GoodId = source.Id;
            dest.Name = source.Name;
            dest.Description = source.Description;
            dest.Price = source.Price;
            dest.PricePerWeek = source.PricePerWeek;
            dest.PricePerMonth = source.PricePerMonth;
            dest.RentPeriodDay = source.RentPeriodDay;
            dest.RentPeriodWeek = source.RentPeriodWeek;
            dest.RentPeriodMonth = source.RentPeriodMonth;
            dest.AgreeToDeliver = source.AgreeToDeliver;
            //dest.Location = source..Location;
            dest.Latitude = source.GoodLocation.Latitude;
            dest.Longitude = source.GoodLocation.Longitude;
            if (source.GoodImages != null && source.GoodImages.Any())
            {
                var img = source.GoodImages.FirstOrDefault(i => i.Type == (int)ImageType.Original && i.Sequence == 0);                if (img != null)                {                    dest.Image = img.FileName;                    if (!String.IsNullOrEmpty(dest.Image))                    {                        dest.Image = dest.Image.ImageUrl(ImageFolder.Good.ToString());                    }                }
            }
            return dest;
        }
    }
}