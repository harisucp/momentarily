using System.Collections.Generic;
using Momentarily.Entities.Entities;
namespace Momentarily.ViewModels.Models
{
    public class CreateMomentarilyItemViewModel
    {
        public IList<KeyValuePair<int, string>> Types { get; set; }
        public IList<KeyValuePair<int, string>> Categories { get; set; }
        public MomentarilyItem Item { get; set; }
    }
}