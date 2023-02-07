using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Apeek.Common;
using Apeek.Entities.Entities;
using Apeek.Test.Common;
using Momentarily.Common.Definitions;
using Momentarily.Entities.Entities;
using Momentarily.UI.Service.Services;
using Momentarily.ViewModels.Models;
using NUnit.Framework;
namespace Momentarily.Test.Core.Service.Impl
{
    [TestFixture]
    public class MomentarilyItemDataServiceTest : MomentarilyBaseTest
    {
        [Test]
        public void CreateMomentarilyItem_MomentarilyItemCreated_Successfuly_Test()
        {
            IMomentarilyItemDataService momentarilyItemDataService = Ioc.Get<IMomentarilyItemDataService>();
            IMomentarilyItemTypeService momentarilyTypes = Ioc.Get<IMomentarilyItemTypeService>();
            var isCreated = false;
            MomentarilyItem newMomentarilyItem = null;
            try
            {
                newMomentarilyItem = new MomentarilyItem();
                newMomentarilyItem.Name = MomentarilyTestConstants.testMomentarilyItemNameSuffix;
                //newMomentarilyItem.TypeId = momentarilyTypes.GetAllTypes()[0].Key;
                //newMomentarilyItem.Location = "Location";
                newMomentarilyItem.CategoryId = 1;
                var result = momentarilyItemDataService.SaveUserItem(newMomentarilyItem, 0);
                isCreated = result.CreateResult == CreateResult.Success;
                if (isCreated)
                {
                    newMomentarilyItem = result.Obj;
                }
            }
            catch (Exception ex)
            {
            }
            finally
            {
                if (isCreated && newMomentarilyItem != null)
                {
                    momentarilyItemDataService.DeleteGood(newMomentarilyItem.Id);
                }
            }
            Assert.True(isCreated);
        }
        [Test]
        public void CreateUsersMomentarilyItem_MomentarilyItemCreated_GetCount_Test()
        {
            int userId = int.MaxValue;
            IMomentarilyItemDataService momentarilyItemDataService = Ioc.Get<IMomentarilyItemDataService>();
            IMomentarilyItemTypeService momentarilyTypes = Ioc.Get<IMomentarilyItemTypeService>();
            var isOneCrated = false;
            MomentarilyItem newMomentarilyItem = null;
            try
            {
                var beforeCreated = momentarilyItemDataService.GetUsersItems(userId).Count();
                newMomentarilyItem = new MomentarilyItem();
                newMomentarilyItem.Name = MomentarilyTestConstants.testMomentarilyItemNameSuffix;
                //newMomentarilyItem.TypeId = momentarilyTypes.GetAllTypes()[0].Key;
                //newMomentarilyItem.Location = "Location";
                newMomentarilyItem.CategoryId = 1;
                var result = momentarilyItemDataService.SaveUserItem(newMomentarilyItem, userId);
                var afterCreated = momentarilyItemDataService.GetUsersItems(userId).Count();
                isOneCrated = afterCreated - beforeCreated == 1;
            }
            catch (Exception ex)
            {
            }
            finally
            {
                if (newMomentarilyItem.Id != 0)
                {
                    momentarilyItemDataService.DeleteGood(newMomentarilyItem.Id);
                }
            }
            Assert.True(isOneCrated);
        }
        private MomentarilyItem CreateItem(string name = null, int? typeId = null, 
            string location = null, int? categoryId = null, double? price = null, 
            DateTime? dateStart = null, DateTime? dateEnd = null,
            double? latitude = null, double? longitude = null)
        {
            var itemService = Ioc.Get<IMomentarilyItemDataService>();
            var item = new MomentarilyItem
            {
                Name = name + MomentarilyTestConstants.testMomentarilyItemNameSuffix,
                //TypeId = typeId ?? 1,
                //Location = location ?? "Test_Location",
                CategoryId = categoryId ?? 1,
                Price = price ?? 0,
                StartEndDate = new GoodStartEndDate
                {
                    DateStart = dateStart,
                    DateEnd = dateEnd
                },
                GoodLocation = new GoodLocation
                {
                    Latitude = latitude ?? 0,
                    Longitude = longitude ?? 0
                }
            };
            item.GoodPropertyValues.Add(MomentarilyItemProperties.MomentarilyItemType,
                new GoodPropertyValue() {PropertyValueDefinitionId = typeId ?? 1});
            item.GoodPropertyValues.Add(MomentarilyItemProperties.MomentarilyItemLocation,
               new GoodPropertyValue() { Value = location ?? "Test_Location" });
            return itemService.SaveUserItem(item, 0).Obj;
        }
        private void DeleteItem(MomentarilyItem item)
        {
            var itemService = Ioc.Get<IMomentarilyItemDataService>();
            if (item != null && item.Id != 0)
            {
                itemService.DeleteGood(item.Id);
            }
        }
        [Test]
        public void GetFilteredItems_FilteredGood_Test()
        {
            var itemService = Ioc.Get<IMomentarilyItemDataService>();
            var searchModel1 = new MomentarilyItemSearchModel
            {
                PriceFrom = 99980,
                PriceTo = 99985,
                Latitude = -68.237637,
                Longitude = 69.307251,
                DateStart = null,
                DateEnd = null
            };
            var searchModel2 = new MomentarilyItemSearchModel
            {
                TypeId = 1,
                CategoryId = 1,
                PriceFrom = 99991,
                Latitude = -68.237637,
                Longitude = 69.307251,
                DateStart = null,
                DateEnd = null
            };
            var searchModel3 = new MomentarilyItemSearchModel
            {
                Latitude = -68.237637,
                Longitude = 69.307251,
                DateStart = null,
                DateEnd = null
            };
            MomentarilyItem itemOne = null;
            MomentarilyItem itemTwo = null;
            MomentarilyItem itemThree = null;
            bool isGoodFilter1 = false;
            bool isGoodFilter2 = false;
            bool isGoodFilter3 = false;
            try
            {
                itemOne = CreateItem("itemOne", price: 99982, latitude: -68.232637, longitude: 69.302251);
                itemTwo = CreateItem("itemTwo", 1, categoryId: 1, price: 99999, latitude: -68.232637, longitude: 69.302251);
                itemThree = CreateItem("itemThree", price: 99985, latitude: -68.239637, longitude: 69.309251);
                var result1 = itemService.GetFilteredItems(searchModel1);
                if (result1.CreateResult == CreateResult.Success && result1.Obj.Goods.Count == 2)
                {
                    isGoodFilter1 = true;
                }
                var result2 = itemService.GetFilteredItems(searchModel2);
                if (result2.CreateResult == CreateResult.Success && result2.Obj.Goods.Count == 1)
                {
                    isGoodFilter2 = true;
                }
                var result3 = itemService.GetFilteredItems(searchModel3);
                if (result3.CreateResult == CreateResult.Success && result3.Obj.Goods.Count == 3)
                {
                    isGoodFilter3 = true;
                }
            }
            finally
            {
                DeleteItem(itemOne);
                DeleteItem(itemTwo);
                DeleteItem(itemThree);
            }
            Assert.IsTrue(isGoodFilter1);
            Assert.IsTrue(isGoodFilter2);
            Assert.IsTrue(isGoodFilter3);
        }
    }
}
