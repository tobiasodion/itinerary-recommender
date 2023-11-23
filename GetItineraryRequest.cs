using Newtonsoft.Json;

namespace az_function
{
    public class GetItineraryRequest
    {
        [JsonProperty(Required = Required.Always)]
        public string FirstName { set; get; }
        [JsonProperty(Required = Required.Always)]
        public string LastName { set; get; }
        [JsonProperty(Required = Required.Always)]
        public string City { set; get; }
        [JsonProperty(Required = Required.Always)]
        public string Email { set; get; }
    }
}