namespace Apeek.Core.RazorRenderEngine
{
    public interface IRazorEngine
    {
        void RegisterTemplate<TModel>(string templateString);
        void RegisterTemplateByPath<TModel>(string templatePath);
        string Render<TModel>(TModel model);
        string Render<TModel>(TModel model, string templateName);
        void CompileTemplates();
    }
}
