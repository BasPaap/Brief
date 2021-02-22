using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bas.Brief.ItemGenerators
{
    public abstract class ItemGenerator
    {
        public string Content { get; private init; }
        public Dictionary<string, string> Parameters { get; private init; }
        
        public abstract Task<string> ToHtmlAsync();
                
        public ItemGenerator(IEnumerable<KeyValuePair<string, string>> parameters, string content)
        {
            Parameters = new Dictionary<string, string>(parameters);
            Content = content;
        }
    }
}
