using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bas.Brief.ItemGenerators
{
    public sealed class Bl3pGenerator : ItemGenerator
    {
        public Bl3pGenerator(IEnumerable<KeyValuePair<string, string>> parameters, string content, bool isFirst, bool isLast)
            : base(parameters, content, isFirst, isLast)
        {
        }

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
