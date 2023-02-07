﻿using Apeek.Common.Interfaces;
using Apeek.Entities.Entities;
using Momentarily.ViewModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apeek.Core.Services
{
    public interface IStockDataService : IDependency
    {
        StockVM GetStockDetail(int id, int? QuantityLeft);
        List<StockMasterDetailVM> GetAllStockMasterDetail();
        StockMasterDetailVM GetStockMasterDetail(int goodid);
        StockMasterDetailVM GetStockMasterDetailWithoutUOW(int goodid);
        bool DeleteStockDetail(int id);
        bool UpdateStockDetail(StockVM model);
    }
}