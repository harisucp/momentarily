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
    public class CovidControllerHelper
    {
        protected readonly IHealthDataService _healthDataService;
        public CovidControllerHelper(IHealthDataService healthDataService)
        {
            _healthDataService = healthDataService;
        }

        public List<CovidGoodViewModel> GetAllCovidGoods()
        {
            try
            {
                Ioc.Get<IDbLogger>().LogMessage(LogSource.HealthDataHelper, string.Format("Trying to get All covid goods from covid helper"));
                var list = _healthDataService.GetAllCovidGoods();
                return list;
            }
            catch (Exception ex)
            {
                Ioc.Get<IDbLogger>().LogMessage(LogSource.HealthDataHelper, string.Format("Failed to get All covid goods from covid helper"));
                throw ex;
            }
        }

        public CovidGoodViewModel GetCovidGood(int id)
        {
            try
            {
                Ioc.Get<IDbLogger>().LogMessage(LogSource.HealthDataHelper, string.Format("Trying to get covid good from covid helper"));
                var list = _healthDataService.GetCovidGood(id);
                return list;
            }
            catch (Exception ex)
            {
                Ioc.Get<IDbLogger>().LogMessage(LogSource.HealthDataHelper, string.Format("Failed to get covid good from covid helper"));
                throw ex;
            }
        }

        public CovidGoodOrder SaveCovidGoodOrder(CovidGoodViewModel model)
        {
            try
            {
                Ioc.Get<IDbLogger>().LogMessage(LogSource.HealthDataHelper, string.Format("Trying to save covid good from covid helper"));
                var result = _healthDataService.SaveCovidGoodOrder(model);
                return result;
            }
            catch (Exception ex)
            {
                Ioc.Get<IDbLogger>().LogMessage(LogSource.HealthDataHelper, string.Format("Failed to save covid good from covid helper"));
                throw ex;
            }
        }
        public CovidOrderPaymentDetail SaveCovidOrderPaymentDetail(CovidOrderPaymentDetailViewModel model)
        {
            try
            {
                Ioc.Get<IDbLogger>().LogMessage(LogSource.HealthDataHelper, string.Format("Trying to save covid order payment detail from covid helper"));
                var result = _healthDataService.SaveCovidOrderPaymentDetail(model);
                return result;
            }
            catch (Exception ex)
            {
                Ioc.Get<IDbLogger>().LogMessage(LogSource.HealthDataHelper, string.Format("Failed to save covid order payment detail from covid helper"));
                throw ex;
            }
        }
    }
}
