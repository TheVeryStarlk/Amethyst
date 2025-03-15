using System.Diagnostics.CodeAnalysis;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Amethyst.Abstractions.Messages;

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
            Converters = { new JsonStringEnumConverter<Color>(JsonNamingPolicy.SnakeCaseLower) }
        };

    [UnconditionalSuppressMessage("Trimming", "IL2026", Justification = "https://github.com/dotnet/runtime/issues/51544#issuecomment-1516232559")]
    [UnconditionalSuppressMessage("AOT", "IL3050", Justification = "https://github.com/dotnet/runtime/issues/51544#issuecomment-1516232559")]
    public static string Serialize<T>(this T instance)
    {
        return JsonSerializer.Serialize(instance, Options);
    }
}