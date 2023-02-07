using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Apeek.Common;
using Apeek.Core.Services;
using Apeek.NH.Repository.Common;
using Apeek.Entities.Entities;
using Apeek.NH.DataAccessLayer.DataAccess;
using NHibernate.Criterion;
namespace Apeek.Test.Common.Utils
{
    public class TestGood
    {
        public static void AddTestGoodFields()
        {
            int userId =0;
             Uow.Wrap(u =>
             {
                 var goodPropertyTypeRep = Ioc.Get<IRepositoryAudit<GoodPropertyType>>();
                 var goodPropertyRep = Ioc.Get<IRepositoryAudit<GoodProperty>>();
                 var goodPropertyValueDefinitionRep = Ioc.Get<IRepositoryAudit<GoodPropertyValueDefinition>>();
                 // add string property type;
                 var stringTestFiledType = new GoodPropertyType(){ Name = TestConstants.testGoodPropertyTypeName};
                 stringTestFiledType = goodPropertyTypeRep.SaveOrUpdateAudit(stringTestFiledType, userId);
                 var stringTestField = new GoodProperty()
                 {
                     Name = TestConstants.testGoodPropertyName,
                     TypeId = stringTestFiledType.Id
                 };
                 goodPropertyRep.SaveOrUpdateAudit(stringTestField, userId);
                 //add selected GoodTestProperty;
                 var selectedTestFiledType = new GoodPropertyType(){ Name = TestConstants.selectedGoodPropertyTypeName};
                 selectedTestFiledType = goodPropertyTypeRep.SaveOrUpdateAudit(selectedTestFiledType, userId);
                 var selectedTestField = new GoodProperty()
                 {
                     Name = TestConstants.selectedGoodPropertyName,
                     TypeId = selectedTestFiledType.Id
                 };
                 selectedTestField  = goodPropertyRep.SaveOrUpdateAudit(selectedTestField,userId);
                 //add GoodTestProperty values
                 var oneValue = new GoodPropertyValueDefinition()
                 {
                     GoodPropertyId = selectedTestField.Id,
                     Name = "SelectName1",
                     Value = "SelectValue1"
                 };
                 oneValue = goodPropertyValueDefinitionRep.SaveOrUpdateAudit(oneValue, userId);
               }, null, Apeek.Common.Logger.LogSource.DAL);
        }
        public static void RemoveTestGoodAndFieldsBySuffix()
        {
            Uow.Wrap(u =>
               {
                   var goodRep = Ioc.Get<IRepositoryAudit<Good>>();
                   var goods = goodRep.Table.Where(g => g.Name.EndsWith(TestConstants.testGoodNameSuffix)).ToList();
                   var ids = goods.Select(g => g.Id);
                   var goodValueRep = Ioc.Get<IRepositoryAudit<GoodPropertyValue>>();
                   var goodValues = goodValueRep.Table.Where(gv => ids.Contains(gv.Id)).ToList();
                   goodValueRep.Delete(goodValues);
                   goodRep.Delete(goods);
                   var testFieldsRepository = Ioc.Get<IRepository<GoodProperty>>();
                   var fields = testFieldsRepository.Table.Where(p => p.Name.EndsWith(TestConstants.testFieldNameSuffix)).ToList();
                   ids = fields.Select(p => p.Id).ToList();
                   var testPropertyValueDefinitionRepository = Ioc.Get<IRepository<GoodPropertyValueDefinition>>();
                   var valueDefinitions =
                       testPropertyValueDefinitionRepository.Table.Where(vd => ids.Contains(vd.GoodPropertyId)).ToList();
                   testPropertyValueDefinitionRepository.Delete(valueDefinitions);
                   testFieldsRepository.Delete(fields);
                   var testFieldTypeRepository = Ioc.Get<IRepository<GoodPropertyType>>();
                   var fieldsType =
                       testFieldTypeRepository.Table.Where(p => p.Name.EndsWith(TestConstants.testFieldTypeNameSuffix))
                           .ToList();
                   testFieldTypeRepository.Delete(fieldsType);
               }, null, Apeek.Common.Logger.LogSource.DAL);
        } 
    }
}
