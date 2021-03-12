﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kuna.Net.Objects
{
    public  class KunaCurrency
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("has_memo")]
        public bool? HasMemo { get; set; }

        [JsonProperty("icons")]
        public Icons Icons { get; set; }

        [JsonProperty("coin")]
        public bool Coin { get; set; }

        [JsonProperty("explorer_link")]
        public string ExplorerLink { get; set; }

        [JsonProperty("sort_order")]
        public long SortOrder { get; set; }

        [JsonProperty("precision")]
        public Precision Precision { get; set; }

        [JsonProperty("privileged")]
        public bool Privileged { get; set; }

        [JsonProperty("fuel")]
        public bool? Fuel { get; set; }
    }

    public  class Icons
    {
        [JsonProperty("std")]
        public Uri Std { get; set; }

        [JsonProperty("xl")]
        public Uri Xl { get; set; }

        [JsonProperty("png_2x")]
        public Uri Png2X { get; set; }

        [JsonProperty("png_3x")]
        public Uri Png3X { get; set; }
    }

    public  class Precision
    {
        [JsonProperty("real")]
        public long Real { get; set; }

        [JsonProperty("trade")]
        public long Trade { get; set; }
    }
}
