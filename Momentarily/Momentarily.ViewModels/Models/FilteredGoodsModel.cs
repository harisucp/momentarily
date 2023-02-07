using System.Collections.Generic;
namespace Momentarily.ViewModels.Models
{
    public class FilteredGoodsModel
    {
        public int Count { get; set; }
        public List<MomentarilyItemMapViewModel> Goods { get; set; }
    }
}
