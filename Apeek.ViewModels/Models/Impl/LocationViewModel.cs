using Apeek.Entities.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apeek.ViewModels.Models
{
    public class LocationViewModel
    {
        public string Location { get; set; }
        public double? SharerLatitude { get; set; }
        public double? SharerLongitude { get; set; }
        public double? BorrowerLatitude { get; set; }
        public double? BorrowerLongitude { get; set; }
        public string LocationId { get; set; }
        public string DeliverBy { get; set; }
        public bool? RideStarted { get; set; }
        public bool? DeliveryConfirm { get; set; }
        public bool? ReturnConfirm { get; set; }
        public int? RequestId { get; set; }
    }
}
