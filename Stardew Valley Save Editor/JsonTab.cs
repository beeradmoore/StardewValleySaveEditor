using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stardew_Valley_Save_Editor
{
    class JsonTab
    {
        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("path")]
        public string Path { get; set; }

        [JsonProperty("items")]
        public JsonItem[] Items { get; set; }



    }
}
