using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using Amethyst.Components.Messages;

namespace Amethyst.Components;

public static class JsonSerializerExtensions
{
    public static JsonSerializerOptions Options { get; } =
        new()
        {
            AllowTrailingCommas = true,
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            NumberHandling = JsonNumberHandling.AllowReadingFromString,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            Converters =
            {
                new JsonStringEnumConverter<Color>(JsonNamingPolicy.SnakeCaseLower)
            }
        };
}