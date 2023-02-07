using Apeek.Common.Interfaces;
using FluentNHibernate.Cfg;
namespace Apeek.NH.DataAccessLayer.DataAccess
{
    public interface IDbFluentConfigurationManager : IDependency
    {
        FluentConfiguration GetConfiguration();
    }
}