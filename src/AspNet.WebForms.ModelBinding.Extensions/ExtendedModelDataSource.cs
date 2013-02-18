using System.Web.UI;
using System.Web.UI.WebControls;

namespace AspNet.WebForms.ModelBinding.Extensions
{
    public class ExtendedModelDataSource : ModelDataSource
    {
        private ExtendedModelDataSourceView _view;

        public ExtendedModelDataSource(Control dataControl)
            : base(dataControl)
        {
            
        }

        public override ModelDataSourceView View
        {
            get
            {
                if (_view == null)
                {
                    _view = new ExtendedModelDataSourceView(this);
                }
                return _view;
            }
        }
    }
}
