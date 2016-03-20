using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stardew_Valley_Save_Editor
{
    public enum ItemType
    {
        Unknown = 0,
        Number,
        String,
        Boolean,
        Decimal,
        Name,
        Gender,
        Season,

    }
    class JsonItem
    {
        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("path")]
        public string Path { get; set; }

        [JsonProperty("type")]
        [JsonConverter(typeof(StringEnumConverter))]
        public ItemType Type { get; set; }

        [JsonProperty("min")]
        public decimal Min { get; set; } = 0;

        [JsonProperty("max")]
        public decimal Max { get; set; } = decimal.MaxValue;

        [JsonProperty("increment")]
        public decimal Increment { get; set; } = 1;

        [JsonProperty("hint")]
        public string Hint { get; set; }

        [JsonIgnore]
        public int GridRow { get; set; } = -1;

        [JsonIgnore]
        public WeakReference<object> InputReference { get; set; }

        [JsonIgnore]
        public WeakReference<JsonTab> ParentTab { get; set; }

    }
}
