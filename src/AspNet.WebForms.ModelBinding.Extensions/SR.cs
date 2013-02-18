using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;

namespace AspNet.WebForms.ModelBinding.Extensions
{
    internal class SR
    {
        private static readonly Lazy<ResourceManager> _resourceManager = new Lazy<ResourceManager>(() => new ResourceManager("System.Web", typeof(Page).Assembly));

        public static string GetString(string name)
        {
            return _resourceManager.Value.GetString(name, CultureInfo.CurrentCulture);
        }
    }
}
