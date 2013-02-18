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

        protected void Page_Init()
        {
            productsList.EnableModelBindingExtensions();
        }

        #region << Sync >>

        public IQueryable<Category> GetCategoriesQueryable()
        {
            return _db.Categories.Include(c => c.Products.Count).SortBy("Name");
        }

        public IEnumerable<Category> GetCategories(int maximumRows, int startRowIndex, string sortByExpression, out int totalRowCount)
        {
            totalRowCount = _db.Categories.Count();
            return _db.Categories.Include(c => c.Products)
                .SortBy(sortByExpression ?? "Name")
                .Skip(startRowIndex)
                .Take(maximumRows)
                .ToList();
        }

        #endregion

        public async Task<SelectResult> GetCategoriesAsync(int maximumRows, int startRowIndex, string sortByExpression)
        {
            return new SelectResult
            {
                TotalRowCount = await _db.Categories.CountAsync(),
                Results = await _db.Categories.Include(c => c.Products)
                    .SortBy(sortByExpression ?? "Name")
                    .Skip(startRowIndex)
                    .Take(maximumRows)
                    .ToListAsync()
            };
        }
    }
}