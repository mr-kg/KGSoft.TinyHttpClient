using Newtonsoft.Json;

namespace KGSoft.TinyHttpClient.Test.Model;

internal class Data
{
    public int Id { get; set; }

    [JsonProperty("first_name")]
    public string FirstName { get; set; }

    [JsonProperty("last_name")]
    public string LastName { get; set; }
}
