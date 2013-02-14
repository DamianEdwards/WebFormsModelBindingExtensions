﻿using System.Web.UI.WebControls;

namespace WebFormsModelBindingExtensions
{
    public static class DataControlExtensions
    {
        public static void EnableModelBindingExtensions(this DataBoundControl control)
        {
            control.CreatingModelDataSource += (s, e) => e.ModelDataSource = new ExtendedModelDataSource(control);
        }

        public static void EnableModelBindingExtensions(this Repeater repeater)
        {
            repeater.CreatingModelDataSource += (s, e) => e.ModelDataSource = new ExtendedModelDataSource(repeater);
        }
    }
}
