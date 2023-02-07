using System;
using System.Collections.Generic;
using System.Reflection;
namespace Apeek.Core.RazorRenderEngine
{
    public class RazorEngine : IRazorEngine
    {
        private readonly Dictionary<string, RazorTemplateEntry> templateItems = new Dictionary<string, RazorTemplateEntry>();
        private object _lockCompileTemplates = new object();
        private Assembly compiledTemplateAssembly;
        private void ValidateArguments(string templatePath)
        {
            if (compiledTemplateAssembly != null)
                throw new InvalidOperationException("May not register new templates after compiling.");
            if (templatePath == null)
                throw new ArgumentNullException("templatePath");
        }
        public void RegisterTemplate<TModel>(string templateString)
        {
            string templateName = GetTemplateNameFromModel(typeof(TModel));
            templateItems[TranslateKey(typeof(TModel), templateName)] = new RazorTemplateEntry() { ModelType = typeof(TModel), TemplateString = templateString, TemplateName = "Rzr" + Guid.NewGuid().ToString("N") };
        }
        public void RegisterTemplateByPath<TModel>(string templatePath)
        {
            ValidateArguments(templatePath);
            string templateName = GetTemplateNameFromModel(typeof (TModel));
            templateItems[TranslateKey(typeof(TModel), templateName)] = new RazorTemplateEntry() { ModelType = typeof(TModel), TemplatePath = templatePath, TemplateName = "Rzr" + Guid.NewGuid().ToString("N") };
        }
        public void CompileTemplates()
        {
            compiledTemplateAssembly = RazorCompiler.Compile(templateItems.Values);
        }
        public string Render<TModel>(TModel model)
        {
            return Render(model, GetTemplateNameFromModel(typeof(TModel)));
        }
        public string Render<TModel>(TModel model, string templateName)
        {
            //we compile templates just before rendering because jit compilation has to load all referenced assemblies
            if (compiledTemplateAssembly == null)
            {
                lock (_lockCompileTemplates)
                {
                    if (compiledTemplateAssembly == null)
                        CompileTemplates();
                }
            }
            if (templateName == null)
                throw new ArgumentNullException("templateName");
            RazorTemplateEntry entry = null;
            try
            {
                entry = templateItems[TranslateKey(typeof(TModel), templateName)];
            }
            catch (KeyNotFoundException)
            {
                throw new ArgumentOutOfRangeException("No template has been registered under this model or name.");
            }
            var template = (RazorTemplateBase<TModel>)compiledTemplateAssembly.CreateInstance("RazorRenderEngine." + entry.TemplateName + "Template");
            template.Model = model;
            template.Execute();
            var output = template.Output;
            template.Reset();
            return output;
        }
        private string GetTemplateNameFromModel(Type model)
        {
            return string.Format("RZR::{0}", model.Name);
        }
        private string TranslateKey(Type model, string templateName)
        {
            return string.Format("{0}::{1}", model.Name, templateName);
        }
    }
}