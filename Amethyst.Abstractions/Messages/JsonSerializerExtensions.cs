using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Amethyst.Abstractions.Messages;

public static class JsonSerializerExtensions
{
    public static JsonSerializerOptions Options { get; } = new()
    {
        AllowTrailingCommas = true,
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        NumberHandling = JsonNumberHandling.AllowReadingFromString,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        TypeInfoResolver = AmethystJsonSerializerContext.Default,
        Converters = { new JsonStringEnumConverter<Color>(JsonNamingPolicy.SnakeCaseLower) }
    };
}