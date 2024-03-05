using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using Amethyst.Api.Components;

namespace Amethyst.Networking.Packets.Status;

internal sealed class StatusResponsePacket : IOutgoingPacket
{
    public static int Identifier => 0x00;

    public required ServerStatus Status { get; init; }

    private string? serializedStatus;

    public int CalculateLength()
    {
        serializedStatus = Status.Serialize();
        return VariableStringHelper.GetBytesCount(serializedStatus);
    }

    public int Write(ref MemoryWriter writer)
    {
        writer.WriteVariableString(serializedStatus!);
        return writer.Position;
    }
}

internal static class JsonSerializerExtensions
{
    private static JsonSerializerOptions Custom => new JsonSerializerOptions
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
        return JsonSerializer.Serialize(instance, Custom);
    }
}