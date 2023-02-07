using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CGerome.DataAccessLayer.Entities;

namespace CGerome.DataAccessLayer.EntityConverters
{
    public class GoodsToSales
    {
        public static GrSales Map(GrGoods good)
        {
            return new GrSales()
                       {
                           Amount = 1,
                           ByCard = 0,
                           DiscountPrice = good.SellPrice,
                           GeromeId = good.GeromeId,
                           ProductId = good.ProductId,
                           InvoicePrice = Math.Round(good.Price,2),
                           SalePrice = Math.Round(good.SellPrice,2),
                           SaleDate = DateTime.Now                           
                       };
        }
    }
}
