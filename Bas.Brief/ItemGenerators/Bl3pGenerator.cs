using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bas.Brief.ItemGenerators
{
    public sealed class Bl3pGenerator : ItemGenerator
    {
        public override string ToHtml()
        {
            var htmlBuilder = new StringBuilder();
            
            if (IsFirst && !IsLast)
            {
                htmlBuilder.Append("Firstly, ");
            }
            else if (!IsFirst && IsLast)
            {
                htmlBuilder.Append("Finally, ");
            }

            htmlBuilder.Append("Bitcoin has gone up in value by a certain percentage.");

            return htmlBuilder.ToString();
        }
    }
}
