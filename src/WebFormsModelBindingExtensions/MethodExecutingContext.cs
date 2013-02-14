using System.Collections.Generic;

namespace WebFormsModelBindingExtensions
{
    public sealed class MethodExecutingContext : DataMethodFilterContext
    {
        private readonly Dictionary<string, object> _parameters = new Dictionary<string, object>();

        public IDictionary<string, object> Parameters
        {
            get
            {
                return _parameters;
            }
        }
    }
}
