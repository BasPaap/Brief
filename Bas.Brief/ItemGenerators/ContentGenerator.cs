using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bas.Brief.ItemGenerators
{
    [ItemGenerator("Content")]
    public sealed class ContentGenerator : ItemGenerator
    {
        public ContentGenerator(IEnumerable<KeyValuePair<string, string>> parameters, string content, CultureInfo culture)
            : base(parameters, content, culture)
        {
        }

        public override Task<string> ToHtmlAsync()
        {
            return Task.FromResult(Content);
        }
    }
}
