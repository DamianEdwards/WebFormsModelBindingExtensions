using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebFormsModelBindingExtensions
{
    public class ExtendedModelDataSource : ModelDataSource
    {
        private ExtendedModelDataSourceView _view;

        public ExtendedModelDataSource(Control dataControl)
            : base(dataControl)
        {
            PerformAsyncDataAccess = true;
        }

        public virtual bool PerformAsyncDataAccess { get; set; }

        public bool IsAsync
        {
            get { return PerformAsyncDataAccess && DataControl.Page.IsAsync; }
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
