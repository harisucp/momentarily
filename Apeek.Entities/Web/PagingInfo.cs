using System;
namespace Apeek.Entities.Web
{
    public class PagingInfo
    {
        public string CurrentLocationName { get; set; }
        public string CurrentServiceName { get; set; }
        public int CurrentPage { get; set; }
        public int ItemsPerPage { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages
        {
            get { return (int)Math.Ceiling((decimal)TotalItems / ItemsPerPage); }
        } 
    }
}