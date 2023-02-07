using System;
using Momentarily.Common.Definitions;
namespace Momentarily.ViewModels.Models
{
    public class MomentarilyItemSearchModel
    {
        public string Location { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public bool SearchByMap { get; set; }
        public double NeLatitude { get; set; }
        public double NeLongitude { get; set; }
        public double SwLatitude { get; set; }
        public double SwLongitude { get; set; }
        public int? TypeId { get; set; }
        public int? CategoryId { get; set; }
        public DateTime? DateStart { get; set; }
        public DateTime? DateEnd { get; set; }
        public double? PriceFrom { get; set; }
        public double? PriceTo { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int Radius { get; set; }
        public string Keyword { get; set; }
        public RentPeriod RentPeriod { get; set; }   
        public SortBy SortBy {get; set; }    
        public MomentarilyItemSearchModel()
        {
            Page = MomentarilyMapProperties.DefaultPage;
            PageSize = MomentarilyMapProperties.DefaultPageSize;
            Radius = MomentarilyMapProperties.DefaultRadius;
            var timeNow = DateTime.Now;
            Location = MomentarilyMapProperties.DefaultLocation;
            Latitude = MomentarilyMapProperties.DefaultLatitude;
            Longitude = MomentarilyMapProperties.DefaultLongitude;
            DateStart = timeNow;
            DateEnd = timeNow.AddDays(14);
            Keyword = string.Empty;
            RentPeriod = RentPeriod.Any;
            SortBy = SortBy.PriceLowToHigh;
        }
    }
}
