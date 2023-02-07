using System;
using System.Collections.Generic;
using CGerome.DataAccessLayer.Entities;
using CGerome.DataAccessLayer.Repository;

namespace CGerome.EntityConverters
{
    public class GoodsStorageToGoods
    {
        //private double kurs = 1;

        /// <summary>
        /// Create new GrGoods, based on GrGoodsStorage
        /// </summary>
        /// <param name="goodsStorage"></param>
        /// <returns></returns>
        public GrGoods Map(GrGoodsStorage goodsStorage)
        {

            var good = new GrGoods
                           {
                               Brand = goodsStorage.Brand,
                               ColorId = goodsStorage.ColorId,
                               Date = goodsStorage.ModDate,
                               Description = goodsStorage.Description,
                               ManualUpdate = goodsStorage.ManualUpdate,
                               Name = goodsStorage.Name,
                               PrevDate = goodsStorage.CreateDate,
                               Tester = goodsStorage.Tester,
                               Uid = goodsStorage.Uid,
                               Sex = goodsStorage.Sex,

                               Unbox = goodsStorage.Unbox,
                               ProductId = goodsStorage.ProductId,

                               Volume = goodsStorage.Volume,
                               PriceId = goodsStorage.PriceId,
                               Rgb = goodsStorage.Rgb,
                               Unit = goodsStorage.Unit,
                               GeromeId = goodsStorage.GeromeId,
                               GoodsGroup  = goodsStorage.GoodsGroup,
                               Identity = goodsStorage.Identity,
                               Discountable = goodsStorage.Discountable
                           };

            if(Kurs.Loaded())
            {
                if(goodsStorage.ManualUpdate == 1)
                {
                    good.SellPrice = goodsStorage.SellPrice;
                }
                else
                {
                    good.SellPrice =
                        Math.Round(GrMarkUpConfig.MarkupPrice(goodsStorage.Price, Kurs.KursOut, goodsStorage.MarkUpType));
                }

                good.Price = Math.Round(goodsStorage.Price * Kurs.KursIn, 2);
                good.PrevPrice = Math.Round(goodsStorage.PrevPrice*Kurs.KursIn, 2);
            }

            return good;
        }

        /// <summary>
        /// Create new GrGoodsStorage, based on GrGoods
        /// </summary>
        /// <param name="good"></param>
        /// <returns></returns>
        public GrGoodsStorage Map(GrGoods good)
        {
            try
            {
                var goodStorage = new GrGoodsStorage
                {
                    Brand = good.Brand,
                    ColorId = good.ColorId,
                    CreateDate = good.PrevDate,
                    Description = good.Description,
                    ManualUpdate = good.ManualUpdate,
                    Name = good.Name,
                    ModDate = good.Date,
                    Tester = good.Tester,
                    Uid = good.Uid,
                    Sex = good.Sex,

                    Unbox = good.Unbox,
                    ProductId = good.ProductId,
                    Volume = good.Volume,
                    PriceId = good.PriceId,
                    Rgb = good.Rgb,

                    Unit = good.Unit,
                    GoodsGroup = good.GoodsGroup,
                    Identity = good.Identity,
                    Discountable = good.Discountable
                };

                if (Kurs.Loaded())
                {
                    goodStorage.SellPrice = good.SellPrice;
                    goodStorage.Price = Math.Round(good.Price / Kurs.KursIn, 2);
                    goodStorage.PrevPrice = Math.Round(good.PrevPrice / Kurs.KursIn, 2);
                }

                return goodStorage;
            }
            catch (Exception ex)
            {
                
                throw ex;
            }
        }

        /// <summary>
        /// Copy in existing GrGoods all data from GrGoodsStorage
        /// </summary>
        /// <param name="good">existing GrGoods</param>
        /// <param name="goodsStorage">existing GrGoodsStorage</param>
        /// <returns>return changed GrGoods</returns>
        public GrGoods Map(GrGoods good, GrGoodsStorage goodsStorage, bool allowMapPrices)
        {

            good.Brand = goodsStorage.Brand;
            good.ColorId = goodsStorage.ColorId;
            good.Date = goodsStorage.ModDate;
            good.Description = goodsStorage.Description;
            good.GeromeId = goodsStorage.GeromeId;
            good.GoodsGroup = goodsStorage.GoodsGroup;
            good.ManualUpdate = goodsStorage.ManualUpdate;
            good.Name = goodsStorage.Name;
            good.PrevDate = goodsStorage.CreateDate;
            good.Tester = goodsStorage.Tester;
            good.Uid = goodsStorage.Uid;
            good.Sex = goodsStorage.Sex;
            good.Unbox = goodsStorage.Unbox;
            good.ProductId = goodsStorage.ProductId;
            good.Volume = goodsStorage.Volume;
            good.PriceId = goodsStorage.PriceId;
            good.Rgb = goodsStorage.Rgb;           
            good.Unit = goodsStorage.Unit;
            good.Identity = goodsStorage.Identity;
            good.Discountable = goodsStorage.Discountable;


            if (Kurs.Loaded())
            {
                if (goodsStorage.ManualUpdate == 1)
                {
                    good.SellPrice = goodsStorage.SellPrice;
                }
                else
                {
                    if (allowMapPrices)
                    {
                        good.SellPrice =
                            Math.Round(GrMarkUpConfig.MarkupPrice(goodsStorage.Price, Kurs.KursOut,
                                                                  goodsStorage.MarkUpType));
                    }
                }
                if (allowMapPrices)
                {
                    good.Price = Math.Round(goodsStorage.Price * Kurs.KursIn, 2);
                    good.PrevPrice = Math.Round(goodsStorage.PrevPrice * Kurs.KursIn, 2);
                }
            }

            return good;
        }

        public List<GrGoods> Synchronize(List<GrGoodsStorage> goodsStorage, List<GrGoods> goodsPresence)
        {
            List<GrGoods> goods = new List<GrGoods>();

            foreach (GrGoods good in goodsPresence)
            {
                //to generate new persistance object we need call CreateNewBasedOnThis()
                goods.Add(good.CreateNewBasedOnThis());
            }

            foreach (GrGoodsStorage goodStorage in goodsStorage)
            {
                var goodPresence = goods.Find(x => x.GeromeId == goodStorage.GeromeId);

                //if we have goods which are presence we need update some data
                if (goodPresence != null)
                {
                    goodPresence.Date = goodStorage.ModDate;
                    goodPresence.Identity = goodStorage.Identity;
                }
                else
                {
                    goods.Add(Map(goodStorage));
                }
            }

            return goods;
        }

    }
}