using Apeek.Core.Interfaces;
using Apeek.Core.RazorRenderEngine;
using Apeek.Core.Services.Impl;
using Apeek.Core.ViewModelFactories;
using StructureMap.Configuration.DSL;
using Apeek.Entities.Entities;
namespace Apeek.Core.IocRegistry
{
    public class ApeekSingletonRegistry : Registry
    {
        public ApeekSingletonRegistry()
        {
            For(typeof(IShapeFactory)).Use(typeof(ShapeFactory));
            For<ITranslateController>().Singleton().Use<TranslateController>();
            RegisterRazorRenderEngine();
        }
        private void RegisterRazorRenderEngine()
        {
            IRazorEngine generator = new RazorEngine();
            generator.RegisterTemplateByPath<UserSecurityDataChangeRequest>(MessageTemplate.VerifySecuritySataTemlpPath);
            For<IRazorEngine>().Singleton().Use(generator);
        }
    }
}