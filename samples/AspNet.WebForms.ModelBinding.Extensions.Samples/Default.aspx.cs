using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using AspNet.WebForms.ModelBinding.Extensions.Samples.Model;

namespace AspNet.WebForms.ModelBinding.Extensions.Samples
{
    public partial class _Default : Page
    {
        private readonly NorthwindContext _db = new NorthwindContext();

        protected void Page_Init(object sender, EventArgs e)
        {
            productsList.EnableModelBindingExtensions();
        }

        // The return type can be changed to IEnumerable, however to support
        // paging and sorting, the following parameters must be added:
        //     int maximumRows
        //     int startRowIndex
        //     out int totalRowCount
        //     string sortByExpression
        public IQueryable<Product> productsList_GetData()
        {
            return _db.Products;
        }

        private async Task<int> GetCount(IQueryable<Product> query)
        {
            return await query.CountAsync();
        }
    }
}