using System;

namespace AspNet.WebForms.ModelBinding.Extensions
{
    public sealed class MethodExecutedContext : DataMethodFilterContext
    {
        public Exception Exception { get; set; }
        public bool ExceptionHandled { get; set; }
    }
}
