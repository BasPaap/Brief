using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bas.Brief.ItemGenerators
{
    public sealed class RedditGenerator : ItemGenerator
    {
        public RedditGenerator(IEnumerable<KeyValuePair<string, string>> parameters, string content)
            : base(parameters, content)
        {
        }

        public override async Task<string> ToHtmlAsync()
        {
            return string.Empty;
        }
    }
}
