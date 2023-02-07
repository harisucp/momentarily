using Apeek.Common;
using Apeek.Core.Services;
using Apeek.Core.Services.Impl;
using Apeek.Entities.Entities;
using Apeek.NH.Repository.Repositories.Impl;
using NUnit.Framework;
using System;
namespace Apeek.Test.Core.Services.Impl
{
    [TestFixture]
    public class GoodRequestServiceTest
    {
        private IGoodRequestService _goodRequestService { get; set; }
        public GoodRequestServiceTest()
        {
            _goodRequestService = Ioc.Get<IGoodRequestService>();
        }
        [Test]
        public void PriceCalculationTest()
        {
            var priceViewModel =_goodRequestService.CalculatePrice(
                GoodId: 29,
                StartDate: new DateTime(2016, 10, 19),
                EndDate: new DateTime(2016, 10, 23),
                ShippingDistance: 8.9,
                 ApplyForDelivery: false
            );
            Assert.AreEqual(5, priceViewModel.Obj.Days);
            Assert.AreEqual(20, priceViewModel.Obj.PerDayCost);
            Assert.AreEqual(100, priceViewModel.Obj.DaysCost);
            Assert.AreEqual(6, priceViewModel.Obj.CustomerServiceFeeCost);
            Assert.AreEqual(0.5, priceViewModel.Obj.CustomerCharityCost);
            Assert.AreEqual(13.35, priceViewModel.Obj.DiliveryCost);
            Assert.AreEqual(96.5, priceViewModel.Obj.SharerCost);
            Assert.AreEqual(3, priceViewModel.Obj.SharerServiceFeeCost);
            Assert.AreEqual(0.5, priceViewModel.Obj.SharerCharityCost);
        }
    }
}
