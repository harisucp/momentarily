using Apeek.ViewModels.Models.Impl;
using System.Collections.Generic;
namespace Momentarily.ViewModels.Models
{
    public class MomentarilyItemSearchViewModel
    {
        public IList<KeyValuePair<int, string>> Types { get; set; }        
        public List<MomentarilyCategoryModel> Categories { get; set; }
        public MomentarilyItemSearchModel SearchModel { get; set; }
        public FilteredGoodsModel Result { get; set; }
   
    }
}
