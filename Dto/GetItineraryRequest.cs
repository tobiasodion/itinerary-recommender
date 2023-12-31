using Newtonsoft.Json;

namespace az_function
{
    public record GetItineraryRequest
    (
        [JsonProperty(Required = Required.Always)]
        string FirstName,
        [JsonProperty(Required = Required.Always)]
        string LastName,
        [JsonProperty(Required = Required.Always)]
        string City,
        [JsonProperty(Required = Required.Always)]
        string Email
    );
}