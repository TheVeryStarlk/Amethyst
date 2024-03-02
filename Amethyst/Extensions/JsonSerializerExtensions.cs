using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Amethyst.Extensions;

internal static class JsonSerializerExtensions
{
    private static JsonSerializerOptions CustomSerializer =>
        new JsonSerializerOptions
        {
            AllowTrailingCommas = true,
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            NumberHandling = JsonNumberHandling.AllowReadingFromString,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

    public static string Serialize<T>(this T instance)
    {
        return JsonSerializer.Serialize(instance, CustomSerializer);
    }
}