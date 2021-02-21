using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bas.Brief.ItemGenerators
{
    public abstract class ItemGenerator
    {
        public bool IsFirst { get; set; }
        public bool IsLast { get; set; }

        public abstract string ToHtml();
    }
}
