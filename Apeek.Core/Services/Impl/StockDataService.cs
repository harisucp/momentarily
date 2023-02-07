using Apeek.Common;
using Apeek.Common.Definitions;
using Apeek.Common.Logger;
using Apeek.Entities.Entities;
using Apeek.NH.DataAccessLayer.DataAccess;
using Apeek.NH.Repository.Repositories;
using Momentarily.ViewModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Apeek.Core.Services.Impl
{

    public class StockDataService : IStockDataService
    {
        private readonly IRepositoryStockDetail _repStockDetail;
        private readonly IRepositoryCovidGood _repCovidGood;
        private readonly IRepositoryCovidGoodOrder _repCovidGoodOrder;
        public StockDataService(IRepositoryStockDetail repStockDetail, IRepositoryCovidGood repCovidGood,
             IRepositoryCovidGoodOrder repCovidGoodOrder)
        {
            _repStockDetail = repStockDetail;
            _repCovidGood = repCovidGood;
            _repCovidGoodOrder = repCovidGoodOrder;
        }
        public StockVM GetStockDetail(int id, int? QuantityLeft)
        {
            StockVM stockDetails = new StockVM();
            try
            {
                Ioc.Get<IDbLogger>().LogWarning(LogSource.StockDataService, string.Format("Trying to get stock detail"));
                Uow.Wrap(u =>
                {
                    stockDetails = (from detail in _repStockDetail.Table
                                    where detail.Id == id
                                    select new StockVM
                                    {
                                        Id = detail.Id,
                                        CovidGoodId = detail.CovidGoodId,
                                        Quantity = detail.Quantity,
                                        OldQuantity = detail.Quantity,
                                        QuantityLeft = QuantityLeft == null ? 0 : QuantityLeft,
                                        Description = detail.Description,
                                        CreateDate = detail.CreateDate,
                                        CreateBy = detail.CreateBy,
                                        ModDate = detail.ModDate,
                                        ModBy = detail.ModBy
                                    }).FirstOrDefault();
                });
            }
            catch (Exception ex)
            {
                Ioc.Get<IDbLogger>().LogWarning(LogSource.StockDataService, string.Format("Failed to get stock detail:- " + ex));
            }
            return stockDetails;
        }

        public List<StockVM> GetAllStockDetail()        {
            List<StockVM> stockDetails = new List<StockVM>();            try
            {
                Ioc.Get<IDbLogger>().LogWarning(LogSource.StockDataService, string.Format("Trying to All get stock detail"));
                Uow.Wrap(u =>
                {
                    stockDetails = (from stock in _repStockDetail.Table

                                    select new StockVM
                                    {
                                        Id = stock.Id,
                                        CovidGoodId = stock.CovidGoodId,
                                        CovidGoodName = (from good in _repCovidGood.Table where good.Id == stock.CovidGoodId select good.Name).FirstOrDefault(),
                                        Quantity = stock.Quantity,
                                        Description = stock.Description,
                                        CreateBy = Convert.ToInt32(stock.CreateBy),
                                        ModBy = Convert.ToInt32(stock.ModBy),
                                        CreateDate = Convert.ToDateTime(stock.CreateDate),
                                        ModDate = Convert.ToDateTime(stock.ModDate),
                                    }).ToList();
                });
            }
            catch (Exception ex)
            {
                Ioc.Get<IDbLogger>().LogWarning(LogSource.StockDataService, string.Format("Failed to get All  stock detail:-  " + ex));
            }
            return stockDetails;

        }

        public StockDetail SaveStockDetail(StockDetailViewModel model)
        {
            StockDetail result = null;
            try
            {
                Ioc.Get<IDbLogger>().LogWarning(LogSource.StockDataService, string.Format("Trying to save stock detail :- "));
                Uow.Wrap(u =>
                {

                    StockDetail entity = new StockDetail();
                    entity.CovidGoodId = model.CovidGoodId;
                    entity.Quantity = model.Quantity;
                    entity.Description = model.Description;
                    entity.CreateBy = 0;
                    entity.ModBy = 0;
                    entity.CreateDate = DateTime.Now;
                    entity.ModDate = DateTime.Now;
                    _repStockDetail.Save(entity);
                    result = new StockDetail();
                    result = entity;

                });
            }
            catch (Exception ex)
            {
                Ioc.Get<IDbLogger>().LogWarning(LogSource.StockDataService, string.Format("Failed to save stock detail :-  " + ex));
            }
            return result;
        }



        public List<StockMasterDetailVM> GetAllStockMasterDetail()
        {
            List<StockMasterDetailVM> stockMasterDetailVMs = new List<StockMasterDetailVM>();
            try
            {
                Ioc.Get<IDbLogger>().LogWarning(LogSource.StockDataService, string.Format("Trying to get all stock master detail :- "));
                Uow.Wrap(u =>
                {
                    var allgoods = _repCovidGood.Table.ToList();
                    foreach (var good in allgoods)
                    {
                        StockMasterDetailVM data = new StockMasterDetailVM();
                        data.CovidGoodId = good.Id;
                        data.CovidGoodName = good.Name;
                        data.Total = _repStockDetail.Table.Where(x => x.CovidGoodId == good.Id).AsEnumerable().Sum(o => o.Quantity);
                        data.Ordered = _repCovidGoodOrder.Table.Where(x => x.CovidGoodId == good.Id).AsEnumerable().Sum(o => o.Quantity);
                        data.QuantityLeft = data.Total - data.Ordered;
                        data.ModDate = (from det in _repStockDetail.Table
                                        where det.CovidGoodId == good.Id
                                        orderby det.Id descending
                                        select det.ModDate).FirstOrDefault();

                        stockMasterDetailVMs.Add(data);
                    }

                });
            }
            catch (Exception ex)
            {
                Ioc.Get<IDbLogger>().LogWarning(LogSource.StockDataService, string.Format("Failed to get all stock master detail :-  " + ex));
            }
            return stockMasterDetailVMs;
        }

        public StockMasterDetailVM GetStockMasterDetail(int goodid)
        {
            StockMasterDetailVM stockMasterDetailVMs = new StockMasterDetailVM();
            try
            {
                Ioc.Get<IDbLogger>().LogWarning(LogSource.StockDataService, string.Format("Trying to get stock master detail :- "));
                Uow.Wrap(u =>
                {
                    var good = _repCovidGood.Table.Where(x => x.Id == goodid).FirstOrDefault();

                    if (good != null)
                    {
                        stockMasterDetailVMs.CovidGoodId = good.Id;
                        stockMasterDetailVMs.CovidGoodName = good.Name;
                        stockMasterDetailVMs.Total = _repStockDetail.Table.Where(x => x.CovidGoodId == good.Id).AsEnumerable().Sum(o => o.Quantity);
                        stockMasterDetailVMs.Ordered = _repCovidGoodOrder.Table.Where(x => x.CovidGoodId == good.Id).AsEnumerable().Sum(o => o.Quantity);
                        stockMasterDetailVMs.QuantityLeft = stockMasterDetailVMs.Total - stockMasterDetailVMs.Ordered;
                        stockMasterDetailVMs.ModDate = (from det in _repStockDetail.Table
                                                        where det.CovidGoodId == good.Id
                                                        orderby det.Id descending
                                                        select det.ModDate).FirstOrDefault();
                    }

                });
            }
            catch (Exception ex)
            {
                Ioc.Get<IDbLogger>().LogWarning(LogSource.StockDataService, string.Format("Failed to get stock master detail :-  " + ex));
            }
            return stockMasterDetailVMs;
        }


        public StockMasterDetailVM GetStockMasterDetailWithoutUOW(int goodid)
        {
            StockMasterDetailVM stockMasterDetailVMs = new StockMasterDetailVM();
            try
            {
                Ioc.Get<IDbLogger>().LogWarning(LogSource.StockDataService, string.Format("Trying to get stock master detail :- "));

                var good = _repCovidGood.Table.Where(x => x.Id == goodid).FirstOrDefault();

                if (good != null)
                {
                    stockMasterDetailVMs.CovidGoodId = good.Id;
                    stockMasterDetailVMs.CovidGoodName = good.Name;
                    stockMasterDetailVMs.Total = _repStockDetail.Table.Where(x => x.CovidGoodId == good.Id).AsEnumerable().Sum(o => o.Quantity);
                    stockMasterDetailVMs.Ordered = _repCovidGoodOrder.Table.Where(x => x.CovidGoodId == good.Id).AsEnumerable().Sum(o => o.Quantity);
                    stockMasterDetailVMs.QuantityLeft = stockMasterDetailVMs.Total - stockMasterDetailVMs.Ordered;
                    stockMasterDetailVMs.ModDate = (from det in _repStockDetail.Table
                                                    where det.CovidGoodId == good.Id
                                                    orderby det.Id descending
                                                    select det.ModDate).FirstOrDefault();


                }


            }
            catch (Exception ex)
            {
                Ioc.Get<IDbLogger>().LogWarning(LogSource.StockDataService, string.Format("Failed to get stock master detail :-  " + ex));
            }
            return stockMasterDetailVMs;
        }

        public bool DeleteStockDetail(int id)
        {
            bool result = false;
            try
            {
                Ioc.Get<IDbLogger>().LogWarning(LogSource.StockDataService, string.Format("Trying to delete stock detail :- "));
                Uow.Wrap(u =>
                {
                    var stockDetail = _repStockDetail.Table.Where(x => x.Id == id).FirstOrDefault();
                    if (stockDetail != null)
                    {
                        _repStockDetail.Delete(stockDetail);
                        result = true;
                    }
                });
            }
            catch (Exception ex)
            {
                Ioc.Get<IDbLogger>().LogWarning(LogSource.StockDataService, string.Format("Failed to delete stock detail :-  " + ex));
            }
            return result;
        }

        public bool UpdateStockDetail(StockVM model)
        {
            var result = false;
            try
            {
                Ioc.Get<IDbLogger>().LogWarning(LogSource.StockDataService, string.Format("Trying to update stock detail :- "));
                Uow.Wrap(u =>
                {
                    StockDetail entity = new StockDetail();
                    entity.Id = model.Id;
                    entity.CovidGoodId = model.CovidGoodId;
                    entity.Quantity = model.Quantity;
                    entity.Description = model.Description;
                    entity.CreateBy = 0;
                    entity.CreateDate = model.CreateDate;
                    entity.ModBy = 0;
                    entity.ModDate = Convert.ToDateTime(DateTime.Now);
                    _repStockDetail.Update(entity);
                    result = true;

                });
            }
            catch (Exception ex)
            {
                Ioc.Get<IDbLogger>().LogWarning(LogSource.StockDataService, string.Format("Failed to update stock detail :- " + ex));
            }
            return result;
        }
    }
}
