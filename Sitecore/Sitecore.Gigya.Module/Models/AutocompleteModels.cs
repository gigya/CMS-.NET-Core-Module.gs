using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sitecore.Gigya.Module.Models
{
    public class AutocompleteResult
    {
        [JsonProperty("suggestions")]
        public List<AutocompleteSuggestion> Suggestions { get; set; }
    }

    public class AutocompleteSuggestion
    {
        [JsonProperty("value")]
        public string Value { get; set; }
        [JsonProperty("data")]
        public string Data { get; set; }
    }
}