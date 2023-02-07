using CGerome.DataAccessLayer.Entities;
using CGerome.DataAccessLayer.Repository;
using StringHelper = CGerome.DataAccessLayer.Helpers.StringHelper;

namespace CGerome.EntityConverters
{
    public class GoodsStorageToInvoice
    {
        /// <summary>
        /// Create new invoice based on GrGoodsStorage
        /// </summary>
        /// <param name="goodsStorage"></param>
        /// <returns></returns>
        public static GrInvoice Map(GrGoodsStorage goodsStorage)
        {
            var invoice = new GrInvoice()
                       {
                           Amount = 0,
                           GeromeId = goodsStorage.GeromeId,
                           ProductId = goodsStorage.ProductId   
                       };
            if(Kurs.Loaded())
            {
                invoice.Price = GrMarkUpConfig.MarkupPrice(goodsStorage.Price, Kurs.KursIn, goodsStorage.MarkUpType);
            }

            string longName = string.Format("{0} {1} {2} {3} {4} {5} {6}", goodsStorage.Name, goodsStorage.Sex,
                                            goodsStorage.Unit,
                                            goodsStorage.Volume, goodsStorage.Tester, goodsStorage.Unbox,
                                            goodsStorage.ColorId);

            invoice.LongName = StringHelper.RemoveWhiteSpaces(longName);
            return invoice;
        }
    }
}
