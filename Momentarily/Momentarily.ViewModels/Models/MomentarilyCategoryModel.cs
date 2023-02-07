using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Momentarily.ViewModels.Models
{
    public class MomentarilyCategoryModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ImagePath { get; set; }
        public MomentarilyCategoryModel()
        {
            this.Id = -1;
            this.Name = string.Empty;
            this.ImagePath = string.Empty;
        }
    }
}
