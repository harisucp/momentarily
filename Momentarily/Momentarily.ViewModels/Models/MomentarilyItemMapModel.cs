using System;
using System.Collections.Generic;
using Apeek.Common.Extensions;
using Apeek.Entities.Constants;
using Apeek.ViewModels.Models;
using Apeek.ViewModels.Models.Impl;
namespace Momentarily.ViewModels.Models
{
    public class MomentarilyItemMapViewModel : GoodViewModel
    {
        public UserViewModel User { get; set; }
        public int UserId { get; set; }
        public string UserImageFileName { get; set; }
        public string GoodImageFileName { get; set; }
        public string PickUpLoaction { get; set; }
        public string UserImageUrl
        {
            get
            {
                return UserImageFileName.ImageUrl(ImageFolder.User.ToString());
            }
        }
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
        public List<GoodImageViewModel> ListingImages { get; set; }        
        public int ReviewCount { get; set; }
        public decimal Rank { get; set; }
        public ListViewModel<ReviewViewModel> SeekersReviews { get; set; }
        public int RentsCount { get; set; }
        public bool CurrentUserIsOwner { get; set; }
        public bool IsApproved { get; set; }
        public decimal CancelledPercentage { get; set; }
        public List<string> GoodShareDates { get; set; }
        public List<string> GoodBookedDates { get; set; }
        public List<string> GoodBookedDatesUntil { get; set; }
        public List<string> AllStartDates { get; set; }
        public List<string> AllEndDates { get; set; }
    }
}