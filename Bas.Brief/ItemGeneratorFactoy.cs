using Bas.Brief.ItemGenerators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Bas.Brief
{
    static class ItemGeneratorFactoy
    {
        internal static ItemGenerator GetItemGenerator(XName name, IEnumerable<object> parameters, string content, bool isFirst, bool isLast)
        {
            throw new NotImplementedException();
        }
    }
}
