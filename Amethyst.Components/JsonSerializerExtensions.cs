using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using Amethyst.Components.Messages;

namespace Amethyst.Components;

public static class JsonSerializerExtensions
{
    private static JsonSerializerOptions Options { get; } =
        new()
        {
            AllowTrailingCommas = true,
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            NumberHandling = JsonNumberHandling.AllowReadingFromString,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            TypeInfoResolver = AmethystJsonSerializerContext.Default,
            Converters =
            {
                new JsonStringEnumConverter<Color>(JsonNamingPolicy.SnakeCaseLower)
            }
        };

    public static string Serialize<T>(this T value)
    {
#pragma warning disable IL2026
#pragma warning disable IL3050
        return JsonSerializer.Serialize(value, Options);
#pragma warning restore IL3050
#pragma warning restore IL2026
    }
}