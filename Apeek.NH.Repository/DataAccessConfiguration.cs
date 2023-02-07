using System.Reflection;
using Apeek.Common;
using Apeek.Common.Configuration;
using Apeek.Common.Interfaces;
using Apeek.Entities.Interfaces;
using Apeek.NH.DataAccessLayer.DataAccess;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate.Dialect;
namespace Apeek.NH.Repository
{
    public class DbFluentConfigurationManager : IDbFluentConfigurationManager
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
    public class DbFluentConfigurationManagerMysql : IDbFluentConfigurationManager
    {
        public FluentConfiguration GetConfiguration()
        {
            return Fluently.Configure()
                                        .Database(MySQLConfiguration.Standard.ConnectionString(x => x
                                        .Is(AppSettings.GetInstance().ConnectionStringMysql)))
                                        .Mappings(m => m.FluentMappings.AddFromAssemblyOf<IEntity>())
                                        .ExposeConfiguration(c => c.Properties.Add("hbm2ddl.keywords", "none"))
                                        .ExposeConfiguration(c => c.Properties.Add("connection.release_mode", "on_close"));
        }
    }
    public class DbFluentConfigurationManagerSqlCe : IDbFluentConfigurationManager
    {
        public FluentConfiguration GetConfiguration()
        {
            return Fluently.Configure()
                    .Database(MsSqlCeConfiguration.Standard.ConnectionString(AppSettings.GetInstance().SqlCeDataBasePath)
                                                            .Dialect("Tophands.Repository.FixedMsSqlCe40Dialect, Tophands.Repository")
                                                            .Driver("NHibernate.Driver.SqlServerCeDriver"))
                    .Mappings(m => m.FluentMappings.AddFromAssemblyOf<IEntity>())
                    .ExposeConfiguration(c => c.Properties.Add("hbm2ddl.keywords", "none"))
                    .ExposeConfiguration(c => c.Properties.Add("connection.release_mode", "on_close"));
        }
    }
    public class FixedMsSqlCe40Dialect : MsSqlCe40Dialect
    {
        public override bool SupportsVariableLimit
        {
            get
            {
                return true;
            }
        }
    }
}
