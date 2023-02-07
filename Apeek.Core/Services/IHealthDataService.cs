using Apeek.Common.Interfaces;
using Apeek.Entities.Entities;
using Momentarily.ViewModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apeek.Core.Services
{
   public interface IHealthDataService : IDependency
    {
      List<CovidGoodViewModel> GetAllCovidGoods();
       CovidGoodViewModel GetCovidGood(int id);
        List<CovidGoodOrderViewModel> GetAllCovidGoodsOrder();
        CovidGoodOrderViewModel GetCovidGoodsOrder(int id);

        CovidGoodOrder SaveCovidGoodOrder(CovidGoodViewModel model);
        bool UpdateCovidGoodOrder(int id);

      CovidOrderPaymentDetail SaveCovidOrderPaymentDetail(CovidOrderPaymentDetailViewModel model);

    }
}
