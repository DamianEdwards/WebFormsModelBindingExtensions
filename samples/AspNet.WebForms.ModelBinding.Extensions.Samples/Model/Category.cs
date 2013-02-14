using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AspNet.WebForms.ModelBinding.Extensions.Samples.Model
{
    public class Category
    {
        public int ID { get; set; }

        public string Name { get; set; }
        
        public virtual ICollection<Product> Products { get; set; }
    }
}