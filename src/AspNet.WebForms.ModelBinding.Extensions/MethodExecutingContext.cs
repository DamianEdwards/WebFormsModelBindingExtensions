using System.Collections.Generic;

namespace AspNet.WebForms.ModelBinding.Extensions
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
