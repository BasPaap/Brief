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
        private record PriceAtDate
        {
            public DateTime Date { get; init; }
            public Decimal Price { get; init; }
        }

        public BitcoinPriceGenerator(IEnumerable<KeyValuePair<string, string>> parameters, string content, CultureInfo culture)
            : base(parameters, content, culture)
        {
        }

        public override async Task<string> ToHtmlAsync()
        {
            var currentBitcoinPrice = await GetCurrentBitcoinPriceAsync();

            if (!currentBitcoinPrice.HasValue)
            {
                return "Er ging iets mis bij het bepalen van de bitcoinprijs.";
            }

            decimal? priceYesterday = null;
            decimal? priceLastWeek = null;
            UpdatePriceHistory(currentBitcoinPrice, ref priceYesterday, ref priceLastWeek);

            var stringBuilder = new StringBuilder();

            if (!string.IsNullOrWhiteSpace(Content))
            {
                var lines = Content.Split('|');
                stringBuilder.Append(string.Format(lines[0], $"<strong>{currentBitcoinPrice.Value.ToString("c", Culture)}</strong>"));

                if (priceYesterday.HasValue && priceLastWeek.HasValue)
                {
                    stringBuilder.Append(GetPriceHistoryHtml(currentBitcoinPrice, priceYesterday, priceLastWeek, lines[1]));
                }
            }

            return stringBuilder.ToString();
        }

        private static string GetPriceHistoryHtml(decimal? currentBitcoinPrice, decimal? priceYesterday, decimal? priceLastWeek, string secondLine)
        {
            var percentageYesterday = (currentBitcoinPrice.Value - priceYesterday.Value) / priceYesterday.Value;
            var percentageLastWeek = (currentBitcoinPrice.Value - priceLastWeek.Value) / priceLastWeek.Value;

            var percentageYesterdayText = $"<span style=\"color:{(percentageYesterday >= 0 ? "green" : "red")}\"><strong>{percentageYesterday:P1}</strong></span>";
            var percentageLastWeekText = $"<span style=\"color:{(percentageLastWeek >= 0 ? "green" : "red")}\"><strong>{percentageLastWeek:P1}</strong></span>";

            return string.Format(secondLine, percentageYesterdayText, percentageLastWeekText);
        }

        private void UpdatePriceHistory(decimal? currentBitcoinPrice, ref decimal? priceYesterday, ref decimal? priceLastWeek)
        {
            if (PersistentItemData.ContainsKey("PriceHistory"))
            {
                var priceHistory = JsonSerializer.Deserialize<List<PriceAtDate>>(PersistentItemData["PriceHistory"]);

                if (priceHistory.Last().Date != DateTime.Today)
                {
                    priceHistory.Add(new PriceAtDate { Date = DateTime.Today, Price = currentBitcoinPrice.Value });
                }

                priceYesterday = priceHistory.FirstOrDefault(p => p.Date == DateTime.Now.AddDays(-1).Date)?.Price;
                priceLastWeek = priceHistory.FirstOrDefault(p => p.Date == DateTime.Now.AddDays(-7).Date)?.Price;

                var priceHistoryToStore = priceHistory.TakeLast(8).ToList(); // Only remember the last eight days of prices (a week plus one because we need to keep the price from a week ago.)
                PersistentItemData["PriceHistory"] = JsonSerializer.Serialize<List<PriceAtDate>>(priceHistoryToStore);
            }
            else
            {
                var priceHistoryToStore = new List<PriceAtDate>
                {
                    new PriceAtDate { Date = DateTime.Today, Price = currentBitcoinPrice.Value }
                };
                PersistentItemData["PriceHistory"] = JsonSerializer.Serialize<List<PriceAtDate>>(priceHistoryToStore, new JsonSerializerOptions() { WriteIndented = true });
            }
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
