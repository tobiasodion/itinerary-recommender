using Newtonsoft.Json;

namespace az_function
{
    public record SendEmailRequest
    (
        [JsonProperty(Required = Required.Always)]
        string RecipientEmail,
        [JsonProperty(Required = Required.Always)]
        string Subject,
        [JsonProperty(Required = Required.Always)]
        string Body
    );
}