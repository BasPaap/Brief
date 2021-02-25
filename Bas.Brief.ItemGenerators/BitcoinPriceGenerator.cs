using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Bas.Brief.ItemGenerators
{
    [ItemGenerator("BitcoinPrice")]
    public sealed class BitcoinPriceGenerator : ItemGenerator
    {
        public BitcoinPriceGenerator(IEnumerable<KeyValuePair<string, string>> parameters, string content, CultureInfo culture)
            : base(parameters, content, culture)
        {
        }

        public override async Task<string> ToHtmlAsync()
        {
            var currentBitcoinPrice = await GetCurrentBitcoinPriceAsync();

            PersistentItemData["price"] = "test";

            if (!currentBitcoinPrice.HasValue)
            {
                return "Er ging iets mis bij het bepalen van de bitcoinprijs.";
            }

            var stringBuilder = new StringBuilder();
            
            if (!string.IsNullOrWhiteSpace(Content))
            {
                stringBuilder.Append(string.Format(Content, currentBitcoinPrice.Value.ToString("c", Culture)));
            }
            
            return stringBuilder.ToString();
        }

        private async Task<decimal?> GetCurrentBitcoinPriceAsync()
        {
            const string blockchainComTickerApiUrl = "https://blockchain.info/ticker";
            
            using var httpClient = new HttpClient();
            var jsonResponse = await httpClient.GetStringAsync(blockchainComTickerApiUrl);

            var jsonDocument = JsonDocument.Parse(jsonResponse);
            var symbolProperty = jsonDocument?.RootElement.GetProperty(Parameters["symbol"].ToUpper());
            var lastPrice = symbolProperty?.GetProperty("last").GetDecimal();

            return lastPrice;
        }
    }
}
