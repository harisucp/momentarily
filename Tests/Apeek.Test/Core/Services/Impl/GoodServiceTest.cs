using Apeek.Common;
using Apeek.Core.Services;
using Apeek.Test.Common;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Apeek.Core.Services.Impl;
using Apeek.Entities.Attributies;
using Apeek.Entities.Entities;
using Apeek.NH.DataAccessLayer.DataAccess;
using Apeek.NH.Repository.Common;
using Apeek.NH.Repository.Repositories;
using Apeek.NH.Repository.Repositories.Impl;
using Apeek.ViewModels.Models;
namespace Apeek.Test.Core.Services.Impl
{
    [TestFixture]
    public class GoodServiceTest : BaseTest
    {
        public override void SetUp()
        {
            base.SetUp();
            Apeek.Test.Common.Utils.TestGood.RemoveTestGoodAndFieldsBySuffix();
            Apeek.Test.Common.Utils.TestGood.AddTestGoodFields();
        }
        public override void TearDown()
        {
            base.TearDown();
        }
        [Test]
        public void CreateGood_GoodCreated_Successfully_Test()
        {
            IGoodService<Good, Good> goodService = null;
            IRepositoryGoodPropertyValueDefinition propertyValuesRepository = Ioc.Get<IRepositoryGoodPropertyValueDefinition>();
            bool isSave = false;
            bool isRead = false;
            Good good = null;
            try
            {
                Ioc.Add(exp =>
                {
                    exp.For<IGoodService<Good, Good>>().Use<GoodService<Good, Good>>();
                });
                IList<GoodPropertyValueDefinition> propertyValues = null;
                Uow.Wrap(u =>
             {
                 propertyValues = propertyValuesRepository.GetValuesByPropertyName(TestConstants.selectedGoodPropertyName);
             }, null, Apeek.Common.Logger.LogSource.DAL);
                goodService = Ioc.Get<IGoodService<Good, Good>>();
                good = new Good();
                good.CategoryId = 1;
                good.Name = "GoodName" + TestConstants.testGoodNameSuffix;
                //good.TestField = "TEST";
                //good.TestValueDefinitionField = propertyValues[0].Id;
                var res = goodService.SaveGood(good, 0);
                var testGoodFromDB = goodService.GetMyGood(0, good.Id);
                if (res.CreateResult == CreateResult.Success)
                {
                    isSave = true;
                }
            }
            finally
            {
                if (good.Id != 0 && goodService != null)
                {
                    goodService.DeleteGood(good.Id);
                }
            }
            Assert.True(isSave);
        }
    }
}
