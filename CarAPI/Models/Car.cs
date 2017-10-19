using Newtonsoft.Json;

namespace CarAPI.Models
{
    public class Car
    {
        [JsonProperty(PropertyName = "id")]
        public string Id
        { get; set; }

        [JsonProperty(PropertyName = "make")]
        public string Make
        { get; set; }

        [JsonProperty(PropertyName = "model")]
        public string Model
        { get; set; }
    }
}