using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Bas.Brief.ItemGenerators
{
    public sealed class RssGenerator : ItemGenerator
    {
        public RssGenerator(IEnumerable<KeyValuePair<string, string>> parameters, string content, bool isFirst, bool isLast)
            : base(parameters, content, isFirst, isLast)
        {
        }

        public override async Task<string> ToHtmlAsync()
        {
            using var httpClient = new HttpClient();
            var dinges = await httpClient.GetStringAsync(Parameters["url"]);

            throw new NotImplementedException();
        }
    }
}
