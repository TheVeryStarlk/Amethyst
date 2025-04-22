using System.Text.Json.Serialization;

namespace Amethyst.Abstractions.Messages;

[JsonSerializable(typeof(Message))]
internal sealed partial class AmethystJsonSerializerContext : JsonSerializerContext;