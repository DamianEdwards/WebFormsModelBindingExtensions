using System;

namespace WebFormsModelBindingExtensions
{
    public sealed class MethodExecutedContext : DataMethodFilterContext
    {
        public Exception Exception { get; set; }
        public bool ExceptionHandled { get; set; }
    }
}
