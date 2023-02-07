using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Apeek.ViewModels.BaseViewModel;
namespace Apeek.ViewModels.Models.Impl
{
    public class ListViewModel<T> where T:class
    {
        public ListViewModel()
        {
            Items = new List<T>();
        }
        public Pagination Pagination { get; set; }
        public List<T> Items { get; set; }
    }
}
