using Apeek.Common.Extensions;
using Apeek.Entities.Entities;
using Apeek.ViewModels.Models.Impl;
using System.Linq;
namespace Apeek.ViewModels.Mappers.Imp
{
    public class GoodBookingRankMapper : IGoodBookingRankMapper
    {
        public ReviewViewModel Map(GoodBookingRank source, ReviewViewModel dest)
        {
            dest.Date = source.CreateDate;
            dest.ReviewerName = source.Reviewer.FirstName + " " + source.Reviewer.LastName;
            if (source.Reviewer.UserImages != null && source.Reviewer.UserImages.Any())
            {
                dest.ReviewerImage = source.Reviewer.UserImages.MainImageUrlThumb(true);
            }
            dest.Message = source.Message;
            dest.Rank = source.Rank;
            return dest;
        }
    }
}