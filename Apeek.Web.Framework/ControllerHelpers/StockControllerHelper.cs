using Apeek.Common;
using Apeek.Common.Logger;
using Apeek.Core.Services;
using Apeek.Entities.Entities;
using Momentarily.ViewModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apeek.Web.Framework.ControllerHelpers
{
    public class StockControllerHelper
    {
        protected readonly IStockDataService _stockDataService;
        public StockControllerHelper(IStockDataService stockDataService)
        {
            _stockDataService = stockDataService;
        }


        public List<StockVM> GetAllStockDetail()
        {
            try
            {
                Ioc.Get<IDbLogger>().LogMessage(LogSource.StockDataHelper, string.Format("Trying to get All stock detail from stock helper"));
                var list = _stockDataService.GetAllStockDetail();
                return list;
            }
            catch (Exception ex)
            {
                Ioc.Get<IDbLogger>().LogMessage(LogSource.StockDataHelper, string.Format("Failed to get All stock detail goods from stock helper"));
                throw ex;
            }
        }



        public StockDetail SaveStockDetail(StockDetailViewModel model)
        {
            try
            {
                Ioc.Get<IDbLogger>().LogMessage(LogSource.StockDataHelper, string.Format("Trying to save stock detail from stock helper"));
                var result = _stockDataService.SaveStockDetail(model);
                return result;
            }
            catch (Exception ex)
            {
                Ioc.Get<IDbLogger>().LogMessage(LogSource.StockDataHelper, string.Format("Failed to save stock detail from stock helper"));
                throw ex;
            }
        }

        public List<StockMasterDetailVM> GetAllStockMasterDetail()
        {
            try
            {
                Ioc.Get<IDbLogger>().LogMessage(LogSource.StockDataHelper, string.Format("Trying to get all stock master detail from stock helper"));
                var result = _stockDataService.GetAllStockMasterDetail();
                return result;
            }
            catch (Exception ex)
            {
                Ioc.Get<IDbLogger>().LogMessage(LogSource.StockDataHelper, string.Format("Failed to get all stock master detail from stock helper"));
                throw ex;
            }
        }

        public StockMasterDetailVM GetStockMasterDetail(int id)
        {
            try
            {
                Ioc.Get<IDbLogger>().LogMessage(LogSource.StockDataHelper, string.Format("Trying to get stock master detail from stock helper"));
                var result = _stockDataService.GetStockMasterDetail(id);
                return result;
            }
            catch (Exception ex)
            {
                Ioc.Get<IDbLogger>().LogMessage(LogSource.StockDataHelper, string.Format("Failed to get stock master detail from stock helper"));
                throw ex;
            }
        }
        public bool DeleteStockDetail(int id)
        {
            try
            {
                Ioc.Get<IDbLogger>().LogMessage(LogSource.StockDataHelper, string.Format("Trying to delete stock detail from stock helper"));
                var result = _stockDataService.DeleteStockDetail(id);
                return result;
            }
            catch (Exception ex)
            {
                Ioc.Get<IDbLogger>().LogMessage(LogSource.StockDataHelper, string.Format("Failed to delete stock detail from stock helper"));
                throw ex;
            }
        }

        public bool UpdateStockDetail(StockVM model)
        {
            try
            {
                Ioc.Get<IDbLogger>().LogMessage(LogSource.StockDataHelper, string.Format("Trying to update stock detail from stock helper"));
                var result = _stockDataService.UpdateStockDetail(model);
                return result;
            }
            catch (Exception ex)
            {
                Ioc.Get<IDbLogger>().LogMessage(LogSource.StockDataHelper, string.Format("Failed to update stock detail from stock helper"));
                throw ex;
            }
        }




        public StockVM GetStockDetail(int id, int? QuantityLeft)
        {
            try
            {
                Ioc.Get<IDbLogger>().LogMessage(LogSource.StockDataHelper, string.Format("Trying to get stock detail from stock helper"));
                var detail = _stockDataService.GetStockDetail(id, QuantityLeft);
                return detail;
            }
            catch (Exception ex)
            {
                Ioc.Get<IDbLogger>().LogMessage(LogSource.StockDataHelper, string.Format("Failed to get stock detail from stock helper"));
                throw ex;
            }
        }

    }
}
