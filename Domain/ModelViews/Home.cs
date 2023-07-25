using System.Text.Json.Serialization;

namespace Identity.ModelViews;
public struct Home
{
    [JsonPropertyName("Mensagem")]
    public string Message { get; set;}
}