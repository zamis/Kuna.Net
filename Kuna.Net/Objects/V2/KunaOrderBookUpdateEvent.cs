﻿using CryptoExchange.Net.Converters;
using Kuna.Net.Converters;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Kuna.Net.Objects.V2
{

    public class KunaOrderBookUpdateEventV2
    {
        [JsonProperty("asks")]
        public List<KunaOrderBookEntryV2> Asks { get; set; }
        [JsonProperty("bids")]
        public List<KunaOrderBookEntryV2> Bids { get; set; }
        [JsonProperty("daily_stats")]
        public DailyStatsV2 DailyStats { get; set; }
    }

    //[JsonConverter(typeof(ArrayConverter))]
    //public class KunaOrderBookUpdateEntry
    //{
    //    [ArrayProperty(0), JsonConverter(typeof(StringToDecimalConverter))]
    //    public decimal Price { get; set; }
    //    [ArrayProperty(1), JsonConverter(typeof(StringToDecimalConverter))]
    //    public decimal Amount { get; set; }
    //    [ArrayProperty(2)]
    //    public int Count { get; set; }

    //}
    public class DailyStatsV2
    {
        [JsonProperty("highest_bid")]
        public decimal? HighestBid { get; set; }

        [JsonProperty("bids_sum")]
        public decimal? BidsSum { get; set; }

        [JsonProperty("lowest_ask")]
        public decimal? LowestAsk { get; set; }

        [JsonProperty("asks_sum")]
        public decimal? AsksSum { get; set; }

        [JsonProperty("price_change")]
        public decimal? PriceChange { get; set; }

        [JsonProperty("price_change_percentage")]
        public decimal? PriceChangePercentage { get; set; }

        [JsonProperty("last_price")]
        public decimal? LastPrice { get; set; }

        [JsonProperty("daily_volume")]
        public decimal DailyVolume { get; set; }

        [JsonProperty("daily_high")]
        public decimal? DailyHigh { get; set; }

        [JsonProperty("daily_low")]
        public decimal? DailyLow { get; set; }
    }

}
