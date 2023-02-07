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
    public class HealthDataService : IHealthDataService
    {
        private readonly IRepositoryCovidGood _repCovidGood;
        private readonly IRepositoryCovidGoodOrder _repCovidGoodOrder;
        private readonly IRepositoryCovidOrderPaymentDetail _repositoryCovidOrderPaymentDetail;
        private readonly  IStockDataService _stockDataService;
        public HealthDataService(IRepositoryCovidGood repCovidGood, IRepositoryCovidGoodOrder repCovidGoodOrder, IRepositoryCovidOrderPaymentDetail repositoryCovidOrderPaymentDetail, IStockDataService stockDataService)
        {
            _repCovidGood = repCovidGood;
            _repCovidGoodOrder = repCovidGoodOrder;
            _repositoryCovidOrderPaymentDetail = repositoryCovidOrderPaymentDetail;
            _stockDataService = stockDataService;
        }
        public List<CovidGoodViewModel> GetAllCovidGoods()        {
            List<CovidGoodViewModel> covidGoods = new List<CovidGoodViewModel>();            try
            {
                Ioc.Get<IDbLogger>().LogWarning(LogSource.HealthDataService, string.Format("Trying to All get Covid Good"));
                Uow.Wrap(u =>
                {
                    //covidGoods = _repCovidGood.Table.ToList();
                    covidGoods = (from good in _repCovidGood.Table select new CovidGoodViewModel
                    {
                        CovidGoodId = good.Id,
                        GoodName = good.Name,
                        GoodDescription = good.Description,
                        GoodPrice = good.Price,
                        GoodImage = good.Image,
                        Quantity = 1,
                        stockMasterDetail = _stockDataService.GetStockMasterDetailWithoutUOW(good.Id),
                        TotalPrice = good.Price,
                        BuyerEmailId = "",
                        OrderDescription = "",
                        StatusId = (int)CovidOrderStatus.Pending,
                        Tax = 9.5,
                        DeliveryCharge = 0.0,
                        DeliveryAddress1 = "",
                        DeliveryAddress2 = "",
                        City = "",
                        State = "",
                        Country = "US",
                        ZipCode = "",
                        IgnoreMarketingMails = false
                    }).ToList();
                });
            }
            catch (Exception ex)
            {
                Ioc.Get<IDbLogger>().LogWarning(LogSource.HealthDataService, string.Format("Failed to get All Covid Good:-  " + ex));
            }
            return covidGoods;

        }

        public CovidGoodViewModel GetCovidGood(int id)
        {
            CovidGoodViewModel covidGood = new CovidGoodViewModel();            try
            {
                Ioc.Get<IDbLogger>().LogWarning(LogSource.HealthDataService, string.Format("Trying to get Covid Good"));
                Uow.Wrap(u =>
                {
                    covidGood = (from good in _repCovidGood.Table
                                 where good.Id == id
                                 select new CovidGoodViewModel
                                 {
                                     CovidGoodId = good.Id,
                                     GoodName = good.Name,
                                     GoodDescription = good.Description,
                                     GoodPrice =good.Price,
                                     GoodImage = good.Image,
                                     Quantity = 1,
                                     TotalPrice = good.Price,
                                     BuyerEmailId = "",
                                     OrderDescription = "",
                                     StatusId = (int)CovidOrderStatus.Pending,
                                     Tax = 9.5,
                                     DeliveryCharge = 0.0,
                                     DeliveryAddress1 = "",
                                     DeliveryAddress2 = "",
                                     City = "",
                                     State = "",
                                     Country = "US",
                                     ZipCode = "",
                                     IgnoreMarketingMails = false
                                 }).FirstOrDefault();
                });
            }
            catch (Exception ex)
            {
                Ioc.Get<IDbLogger>().LogWarning(LogSource.HealthDataService, string.Format("Failed to get Covid Good:-  " + ex));
            }
            return covidGood;
        }

        public List<CovidGoodOrderViewModel> GetAllCovidGoodsOrder()
        {
            List<CovidGoodOrderViewModel> covidGoodOrders = new List<CovidGoodOrderViewModel>();            try
            {
                Ioc.Get<IDbLogger>().LogWarning(LogSource.HealthDataService, string.Format("Trying to get All Covid Good order"));
                Uow.Wrap(u =>
                {
                    covidGoodOrders = (from order in _repCovidGoodOrder.Table
                                       join good in _repCovidGood.Table
                                       on order.CovidGoodId equals good.Id
                                       where order.StatusId != (int)CovidOrderStatus.Closed
                                       select new CovidGoodOrderViewModel
                                       {
                                           Id = order.Id,
                                           CovidGoodId = order.CovidGoodId,
                                           CovidGoodName = good.Name,
                                           OrderPrice = order.OrderPrice,
                                           Quantity = order.Quantity,
                                           TotalPrice = order.TotalPrice,
                                           FullName = order.FullName,
                                           BuyerEmailId = order.BuyerEmailId,
                                           Description = order.Description,
                                           StatusId = order.StatusId,
                                           StatusName = Enum.GetName(typeof(CovidOrderStatus), order.StatusId),
                                           Tax = order.Tax,
                                           DeliveryCharge = order.DeliveryCharge,
                                           DeliveryAddress1 = order.DeliveryAddress1,
                                           DeliveryAddress2 = order.DeliveryAddress2,
                                           City = order.City,
                                           State = order.State,
                                           Country = order.Country,
                                           ZipCode = order.ZipCode,
                                           CreateDate = order.CreateDate,
                                           Moddate = order.ModDate
                                       }).OrderByDescending(x => x.Id).ToList();
                });
            }
            catch (Exception ex)
            {
                Ioc.Get<IDbLogger>().LogWarning(LogSource.HealthDataService, string.Format("Failed to get All Covid Good order:-  " + ex));
            }
            return covidGoodOrders;
        }

        public CovidGoodOrderViewModel GetCovidGoodsOrder(int id)
        {
            CovidGoodOrderViewModel covidGoodOrder = new CovidGoodOrderViewModel();            try
            {
                Ioc.Get<IDbLogger>().LogWarning(LogSource.HealthDataService, string.Format("Trying to get detail of Covid Good order"));
                Uow.Wrap(u =>
                {
                    covidGoodOrder = (from order in _repCovidGoodOrder.Table
                                      join good in _repCovidGood.Table
                                      on order.CovidGoodId equals good.Id
                                      where order.Id == id
                                      select new CovidGoodOrderViewModel
                                      {
                                          Id = order.Id,
                                          CovidGoodId = order.CovidGoodId,
                                          CovidGoodName = good.Name,
                                          OrderPrice = order.OrderPrice,
                                          Quantity = order.Quantity,
                                          TotalPrice = order.TotalPrice,
                                          FullName = order.FullName,
                                          BuyerEmailId = order.BuyerEmailId,
                                          Description = order.Description,
                                          StatusId = order.StatusId,
                                          StatusName = Enum.GetName(typeof(CovidOrderStatus), order.StatusId),
                                          Tax = order.Tax,
                                          DeliveryCharge = order.DeliveryCharge,
                                          DeliveryAddress1 = order.DeliveryAddress1,
                                          DeliveryAddress2 = order.DeliveryAddress2,
                                          City = order.City,
                                          State = order.State,
                                          Country = order.Country,
                                          ZipCode = order.ZipCode,
                                          CreateDate = order.CreateDate,
                                          Moddate = order.ModDate
                                      }).FirstOrDefault();
                });
            }
            catch (Exception ex)
            {
                Ioc.Get<IDbLogger>().LogWarning(LogSource.HealthDataService, string.Format("Failed to get detail of Covid Good order:-  " + ex));
            }
            return covidGoodOrder;

        }


        public CovidGoodOrder SaveCovidGoodOrder(CovidGoodViewModel model)
        {
            CovidGoodOrder result = null;
            try
            {
                Ioc.Get<IDbLogger>().LogWarning(LogSource.HealthDataService, string.Format("Trying to save Covid Good order"));
                Uow.Wrap(u =>
                {

                    CovidGoodOrder entity = new CovidGoodOrder();
                    entity.CovidGoodId = model.CovidGoodId;
                    entity.OrderPrice = model.GoodPrice;
                    entity.Quantity = model.Quantity;
                    entity.TotalPrice = model.TotalPrice;
                    entity.BuyerEmailId = model.BuyerEmailId;
                    entity.Description = model.OrderDescription;
                    entity.StatusId = model.StatusId;
                    entity.Tax = model.Tax;
                    entity.DeliveryCharge = model.DeliveryCharge;
                    entity.DeliveryAddress1 = model.DeliveryAddress1;
                    entity.DeliveryAddress2 = model.DeliveryAddress2;
                    entity.City = model.City;
                    entity.State = model.State;
                    entity.Country = model.Country;
                    entity.ZipCode = model.ZipCode;
                    entity.FullName = model.FullName;
                    entity.CreateBy = 0;
                    entity.ModBy = 0;
                    entity.CreateDate = DateTime.Now;
                    entity.ModDate = DateTime.Now;
                    _repCovidGoodOrder.Save(entity);
                    result = new CovidGoodOrder();
                    result = entity;

                });
            }
            catch (Exception ex)
            {
                Ioc.Get<IDbLogger>().LogWarning(LogSource.HealthDataService, string.Format("Failed to save Covid Good order:-  " + ex));
            }
            return result;
        }

        public bool UpdateCovidGoodOrder(int id)
        {
            bool result = false;
            try
            {
                Ioc.Get<IDbLogger>().LogWarning(LogSource.HealthDataService, string.Format("Trying to update Covid Good order"));
                Uow.Wrap(u =>
                {
                    var entity = _repCovidGoodOrder.Table.Where(x => x.Id == id).FirstOrDefault();
                    if (entity != null && entity.Id > 0)
                    {
                        entity.StatusId = (int)CovidOrderStatus.Closed;
                        _repCovidGoodOrder.Update(entity);
                    }
                    result = true;

                });
            }
            catch (Exception ex)
            {
                Ioc.Get<IDbLogger>().LogWarning(LogSource.HealthDataService, string.Format("Failed to update Covid Good order:-  " + ex));
            }
            return result;
        }

        public CovidOrderPaymentDetail SaveCovidOrderPaymentDetail(CovidOrderPaymentDetailViewModel model)
        {
            CovidOrderPaymentDetail result = null;
            try
            {
                Ioc.Get<IDbLogger>().LogWarning(LogSource.HealthDataService, string.Format("Trying to save Covid order payment detail"));
                Uow.Wrap(u =>
                {
                    CovidOrderPaymentDetail entity = new CovidOrderPaymentDetail();
                    entity.CovidOrderId = model.CovidOrderId;
                    entity.PayId = model.PayId;
                    entity.Intent = model.Intent;
                    entity.State = model.State;
                    entity.Amount = model.Amount;
                    entity.InvoiceNumber = model.InvoiceNumber;
                    entity.Description = model.Description;
                    entity.CreateTime = model.CreateTime;
                    entity.UpdateTime = model.UpdateTime;
                    entity.PayerEmail = model.PayerEmail;
                    entity.PayerId = model.PayerId;
                    entity.Cart = model.Cart;
                    entity.CreateBy = 0;
                    entity.ModBy = 0;
                    entity.CreateDate = DateTime.Now;
                    entity.ModDate = DateTime.Now;
                    _repositoryCovidOrderPaymentDetail.Save(entity);
                    result = new CovidOrderPaymentDetail();
                    result = entity;
                });
            }
            catch (Exception ex)
            {
                Ioc.Get<IDbLogger>().LogWarning(LogSource.HealthDataService, string.Format("Failed to save Covid order payment detail:-  " + ex));
            }
            return result;
        }
    }
}
