using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.ModelBinding;
using System.Web.UI.WebControls;
using AspNet.WebForms.ModelBinding.Extensions.Samples.Model;

namespace AspNet.WebForms.ModelBinding.Extensions.Samples
{
    public partial class Products : System.Web.UI.Page
    {
        private readonly NorthwindContext _db = new NorthwindContext();

        protected void Page_Init()
        {
            productsList.EnableModelBindingExtensions();
        }

        public async Task<SelectResult> GetProductsAsync([QueryString]int? categoryId, int maximumRows, int startRowIndex, string sortByExpression)
        {
            var products = _db.Products.Include(p => p.Category);

            if (categoryId.HasValue)
            {
                products = products.Where(p => p.Category.ID == categoryId);
            }

            return new SelectResult
            {
                TotalRowCount = await products.CountAsync(),
                Results = await products
                    .SortBy(sortByExpression ?? "Name")
                    .Skip(startRowIndex)
                    .Take(maximumRows)
                    .ToListAsync()
            };
        }

        public async Task<int> DeleteProductAsync(int id)
        {
            _db.Products.Remove(
                _db.Products.Attach(
                    new Product { ID = id }));
            return await _db.SaveChangesAsync();
        }
    }
}