using System;
using System.Reflection;
using System.Collections.Generic;
using Apeek.Entities.Attributies;
using Apeek.Entities.Entities;
using Apeek.Entities.Interfaces;

namespace Apeek.ViewModels.Models
{
    public class GoodViewModel : IAuditEntity
    {
        public GoodViewModel()
        {
            //GoodPropertyValues = new Dictionary<string, GoodPropertyValueViewModel>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public float Price { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ModDate { get; set; }
        public int CreateBy { get; set; }
        public int ModBy { get; set; }
        public int CategoryId { get; set; }

        //todo GoodPropertyValue replace with GoodPropertyValueViewModel
        //public Dictionary<string, GoodPropertyValueViewModel> GoodPropertyValues { get; set; }

        

    }
}