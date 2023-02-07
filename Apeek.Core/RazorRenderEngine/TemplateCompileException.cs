using System;
using System.CodeDom.Compiler;
using System.Text;
using Apeek.Common.Converters;
namespace Apeek.Core.RazorRenderEngine
{
    public class TemplateCompileException : Exception
    {
        public TemplateCompileException(CompilerErrorCollection errors, string sourceCode)
        {
            Errors = errors;
            SourceCode = sourceCode;
        }
        public CompilerErrorCollection Errors { get; private set; }
        public string SourceCode { get; private set; }
        public override string Message
        {
            get
            {
                var str = new StringBuilder();
                foreach (var error in Errors)
                {
                    str.AppendLine(error.ToString());
                }
                return str.ToString();
            }
        }
    }
}
