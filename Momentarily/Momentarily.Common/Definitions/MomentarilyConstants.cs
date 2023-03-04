using System.Collections.Generic;
using Apeek.Common.Interfaces;
using Apeek.Entities.Constants;
using Apeek.Entities.Extensions;
namespace Momentarily.Common.Definitions
{
    public class MomentarilyConstants
    {
    }
    public class MomentarilyImageHelper
    {
        public const string ImageResizerModeCrop = "crop";
        public const string ImageResizerScaleBoth = "both";
    }
    public class MomentarilyImageSettings : IImageSettings
    {
        public List<string> ImageExtensions
        {
            get
            {
                return new List<string> { ".JPG", ".JPEG", ".PNG", ".BMP" };
            }
        }
        public Dictionary<ImageType, ImageUpdateParams> UserImageSizes
        {
            get
            {
                return new Dictionary<ImageType, ImageUpdateParams>
            {
                {ImageType.Thumb, new ImageUpdateParams() {width = "45", height = "45", scale = MomentarilyImageHelper.ImageResizerScaleBoth, mode = MomentarilyImageHelper.ImageResizerModeCrop}},
                //{ImageType.LargeThumb, new ImageUpdateParams() {width = "45", height = "45", scale = MomentarilyImageHelper.ImageResizerScaleBoth, mode = MomentarilyImageHelper.ImageResizerModeCrop}},
                {ImageType.Normal, new ImageUpdateParams() {width = "256", height = "256", scale = MomentarilyImageHelper.ImageResizerScaleBoth, mode = MomentarilyImageHelper.ImageResizerModeCrop}},
                {ImageType.Large, new ImageUpdateParams() {width = "1024", height = "1024", scale = MomentarilyImageHelper.ImageResizerScaleBoth, mode = MomentarilyImageHelper.ImageResizerModeCrop}}
            };
            }
        }
        public Dictionary<ImageType, ImageUpdateParams> GoodImageSizes
        {
            get
            {
                return new Dictionary<ImageType, ImageUpdateParams>
            {
                //{ImageType.Normal, new ImageUpdateParams() {width = "320", height = "320", scale = MomentarilyImageHelper.ImageResizerScaleBoth, mode = MomentarilyImageHelper.ImageResizerModeCrop}},
                //{ImageType.Large, new ImageUpdateParams() {width = "1280", height = "1280", scale = MomentarilyImageHelper.ImageResizerScaleBoth, mode = MomentarilyImageHelper.ImageResizerModeCrop}}
            };
            }
        }
    }
    public class MomentarilyItemProperties
    {
        public const string MomentarilyItemType = "MomentarilyItem_Type";
        public const string MomentarilyItemDeposit = "MomentarilyItem_Deposit";
        public const string MomentarilyItemLocation = "MomentarilyItem_Location";
        public const string MomentarilyItemPickUpLocation = "MomentarilyItem_PickUpLocation";
    }
    public class MomentarilyMapProperties
    {
        public const int DefaultPage = 1;
        public const int DefaultPageSize = 5;
        public const int DefaultRadius = 25;
        public const string DefaultLocation = "";
        public const double DefaultLatitude = 0;
        public const double DefaultLongitude = 0;
    }
    public class MomentarilySystemMessages
    {
        public const string BookingRequest = "<div><b>Booking request (#{0})</b></div> <div><a class=&quoteview-book-btn&quote href=&quote{1}&quote>View Booking Request</a></div>";
        public const string ApproveRequest = "<div><b>Approved request (#{0})</b></div> <div><a class=&quoteview-book-btn&quote href=&quote{1}&quote>View Booking Request</a></div>" +
                                            "<div><a class=&quotepay-for-book-btn&quote href=&quote{2}&quote>Pay for Booking</a></div>";
        public const string DeclineRequest = "<div><b>Declined request (#{0})</b></div> <div><a class=&quoteview-book-btn&quote href=&quote{1}&quote>View Booking Request</a></div><div class=&quotedecline-description&quote>{2}</div>";
        public const string PayRequest = "<div><b>Pay request (#{0})</b></div> <div><a class=&quoteview-book-btn&quote href=&quote{1}&quote>View Booking Request</a></div>";
        public const string DepositRequest = "<div><b>Deposit request (#{0})</b></div><div><a class=&quoteview-book-btn&quote href=&quote{1}&quote>View Booking Request</a></div>";
        public const string SharerDispute = "<div><b>Dispute started for request (#{0})</b></div><div><a class=&quoteview-book-btn&quote href=&quote{1}&quote>View Booking Request</a></div>";
        public const string SeekerDispute = "<div><b>Dispute started for request (#{0})</b></div><div><a class=&quoteview-book-btnk&quote href=&quote{1}&quote>View Booking Request</a></div>";
        public const string SeekerNeedReview = "<div><b>Write a review for booking (#{0})</b></div><div><a class=&quoteview-book-btn&quote href=&quote{1}&quote>View Booking Request</a></div>";
        public const string SharerNeedReview = "<div><b>Write a review for booking (#{0})</b></div><div><a class=&quoteview-book-btn&quote href=&quote{1}&quote>View Booking Request</a></div>";
    }
}
