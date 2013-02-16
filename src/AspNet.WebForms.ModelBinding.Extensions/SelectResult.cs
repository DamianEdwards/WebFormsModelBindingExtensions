using System.Collections;
using System.Collections.Generic;

namespace AspNet.WebForms.ModelBinding.Extensions
{
    public class SelectResult
    {
        public int? TotalRowCount { get; set; }
        public IEnumerable Results { get; set; }
    }
}
