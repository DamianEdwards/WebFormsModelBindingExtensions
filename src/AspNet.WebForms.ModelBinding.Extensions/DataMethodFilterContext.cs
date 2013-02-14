using System.Web.UI;

namespace AspNet.WebForms.ModelBinding.Extensions
{
    public abstract class DataMethodFilterContext
    {
        public Control DataControl { get; set; }
        public DataSourceOperation Operation { get; internal set; }
        public object Result { get; set; }
    }
}
