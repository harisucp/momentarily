using Apeek.Entities.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Momentarily.ViewModels.Models
{
    public class StockDetailViewModel
    {
        public int CovidGoodId { get; set; }
        public int Quantity { get; set; }
        public string Description { get; set; }
        public List<StockVM> stockDetails { get; set; }
        public List<StockMasterDetailVM> stockmasterAllDetails { get; set; }
    }
    public class StockVM
    {
        public int Id { get; set; }
        public int CovidGoodId { get; set; }
        public string CovidGoodName { get; set; }
        public int Quantity { get; set; }
        public int? QuantityLeft { get; set; }
        public int OldQuantity { get; set; }
        public string Description { get; set; }
        public int CreateBy { get; set; }
        public int ModBy { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ModDate { get; set; }

    }

    public class StockMasterDetailVM
    {
        public int CovidGoodId { get; set; }
        public string CovidGoodName { get; set; }
        public int Total { get; set; }
        public int Ordered { get; set; }
        public int QuantityLeft { get; set; }
        public DateTime ModDate { get; set; }

    }
}
