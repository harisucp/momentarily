using System.IO;
using System.Reflection;
using Apeek.Common;
using Apeek.Common.Configuration;
using Apeek.Common.Interfaces;
using Apeek.Entities.Interfaces;
using Apeek.NH.DataAccessLayer.DataAccess;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
namespace Apeek.Test.Common
{
    public class DbFluentConfigurationManagerForTests : IDbFluentConfigurationManager
    {
        public FluentConfiguration GetConfiguration()
        {
            return Fluently.Configure()
                                        .Database(MsSqlConfiguration.MsSql2012.ConnectionString(x => x
                                        .Is(AppSettings.GetInstance().ConnectionStringMysql)))
                                        .Mappings(m => Ioc.Get<IEntityMappingProvider>().GetMapping(m))
                                        .ExposeConfiguration(c => c.Properties.Add("hbm2ddl.keywords", "none"))
                                        .ExposeConfiguration(c => c.Properties.Add("connection.release_mode", "on_close"));
        }
    }
    public class DbFluentConfigurationManagerMySqlForTests : IDbFluentConfigurationManager
    {
        public FluentConfiguration GetConfiguration()
        {
            return Fluently.Configure()
                                        .Database(MySQLConfiguration.Standard.ConnectionString(x => x
                                        .Is(AppSettings.GetInstance().ConnectionStringTest)))
                                        .Mappings(m => m.FluentMappings.AddFromAssemblyOf<IEntity>())
                                        .ExposeConfiguration(c => c.Properties.Add("hbm2ddl.keywords", "none"))
                                        .ExposeConfiguration(c => c.Properties.Add("connection.release_mode", "on_close"));
        }
    }
    public class DbFluentConfigurationManageSqlCerForTests : IDbFluentConfigurationManager
    {
        public FluentConfiguration GetConfiguration()
        {
            return Fluently.Configure()
                           .Database(MsSqlCeConfiguration.Standard.ConnectionString(AppSettings.GetInstance().ConnectionStringTest)
                           .Dialect("Tophands.Repository.FixedMsSqlCe40Dialect, Tophands.Repository"))
                           .Mappings(m => m.FluentMappings.AddFromAssemblyOf<IEntity>())
                           .ExposeConfiguration(c => c.Properties.Add("hbm2ddl.keywords", "none"))
                           .ExposeConfiguration(c => c.Properties.Add("connection.release_mode", "on_close"));
        }
    }
}