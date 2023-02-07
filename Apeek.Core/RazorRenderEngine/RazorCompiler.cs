using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Web.Razor;
using Microsoft.CSharp;
namespace Apeek.Core.RazorRenderEngine
{
    public class RazorCompiler
    {
        private static GeneratorResults GenerateCode(RazorTemplateEntry entry)
        {
            var host = new RazorEngineHost(new CSharpRazorCodeLanguage());
            host.DefaultBaseClass = string.Format("Apeek.Core.RazorRenderEngine.RazorTemplateBase<{0}>", entry.ModelType.FullName);
            host.DefaultNamespace = "RazorRenderEngine";
            host.DefaultClassName = entry.TemplateName + "Template";
            host.NamespaceImports.Add("System");
            GeneratorResults razorResult = null;
            TextReader reader = null;
            if (!string.IsNullOrWhiteSpace(entry.TemplatePath))
                reader = new StreamReader(entry.TemplatePath, Encoding.UTF8);
            else if (!string.IsNullOrWhiteSpace(entry.TemplateString))
                reader = new StringReader(entry.TemplateString);
            razorResult = new RazorTemplateEngine(host).GenerateCode(reader);
            reader.Dispose();
            return razorResult;
        }
        private static CompilerParameters BuildCompilerParameters()
        {
            var @params = new CompilerParameters();
            HashSet<string> asmNames = new HashSet<string>();
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                try
                {
                    if (assembly.ManifestModule.Name != "<In Memory Module>" && !asmNames.Contains(assembly.FullName))
                    {
                        @params.ReferencedAssemblies.Add(assembly.Location);
                        asmNames.Add(assembly.FullName);
                    }
                }
                catch (NotSupportedException)
                {
                }
            }
            @params.GenerateInMemory = true;
            @params.IncludeDebugInformation = false;
            @params.GenerateExecutable = false;
            @params.CompilerOptions = "/target:library /optimize";
            return @params;
        }
        public static Assembly Compile(IEnumerable<RazorTemplateEntry> entries)
        {
            var builder = new StringBuilder();
            var codeProvider = new CSharpCodeProvider();
            using (var writer = new StringWriter(builder))
            {
                foreach (var razorTemplateEntry in entries)
                {
                    var generatorResults = GenerateCode(razorTemplateEntry);
                    codeProvider.GenerateCodeFromCompileUnit(generatorResults.GeneratedCode, writer, new CodeGeneratorOptions());
                }
            }
            var result = codeProvider.CompileAssemblyFromSource(BuildCompilerParameters(), new[] { builder.ToString() });
            if (result.Errors != null && result.Errors.Count > 0)
                throw new TemplateCompileException(result.Errors, builder.ToString());
            return result.CompiledAssembly;
        }
    }
}