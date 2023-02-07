using Apeek.Core.Services;
using Apeek.Core.Services.Impl;
using Apeek.Core.Services.Impl.AWS.S3;
using Apeek.Core.Services.Impl.ExternalProvider.LocalStorage;
using Apeek.NH.Repository.Common;
using Apeek.NH.Repository.Repositories;
using Apeek.NH.Repository.Repositories.Impl;
using StructureMap.Configuration.DSL;
namespace Apeek.Core.IocRegistry
{
    public class CoreIocRegistry : Registry
    {
        public CoreIocRegistry()
        {
            var settings = new SettingsDataService();
            For(typeof(IRepository<>)).Use(typeof(Repository<>));
            For(typeof(IRepositoryAudit<>)).Use(typeof(RepositoryAudit<>));
            For<IExternalFileStorageService>().Use<LocalStorageService>()
                .Ctor<IExternalFileStorageConfig>("externalFileStorageConfig")
                .Is(settings.GetExternalFileStorageConfig());
            For(typeof(IGoodService<,>)).Use(typeof(GoodService<,>));
            For(typeof(IUserDataService<>)).Use(typeof(UserDataService<>));
            For(typeof(IRepositoryGood<,>)).Use(typeof(RepositoryGood<,>));
            For<IPaymentService>().Use<PayPalPaymentService>();
        }
    }
}