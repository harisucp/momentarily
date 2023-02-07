using System;
namespace Apeek.Core.RazorRenderEngine
{
    public class RazorTemplateEntry
    {
        public Type ModelType { get; set; }
        public string TemplateString { get; set; }
        public string TemplatePath { get; set; }
        public string TemplateName { get; set; }
    }
}
