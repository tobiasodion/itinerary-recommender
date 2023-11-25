using Newtonsoft.Json;

namespace az_function
{
    public record GetCompletionRequest
    (
        [JsonProperty(Required = Required.Always)]
        string ApiKey,
        [JsonProperty(Required = Required.Always)]
        string ApiUrl,
        [JsonProperty(Required = Required.Always)]
        string Prompt,
        [JsonProperty(Required = Required.Always)]
        string Model,
        [JsonProperty(Required = Required.Always)]
        int MaxToken
    );
}