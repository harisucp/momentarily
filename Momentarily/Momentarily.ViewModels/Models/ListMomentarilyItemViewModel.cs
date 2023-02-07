using System.Collections.Generic;
using Apeek.Entities.Entities;
using System;
namespace Momentarily.ViewModels.Models
{
    public class ListMomentarilyItemViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double DailyPrice { get; set; }
        public double WeeklyPrice { get; set; }
        public double MounthlyPrice { get; set; }
        public string Image { get; set; }
        public int BookingCount { get; set; }
        public DateTime CreateDate { get; set; }
        public virtual Dictionary<string, GoodPropertyValue> GoodPropertyValues { get; set; }
    }
}