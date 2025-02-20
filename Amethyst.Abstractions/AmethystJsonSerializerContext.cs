using System.Text.Json.Serialization;
using Amethyst.Abstractions.Messages;

namespace Amethyst.Abstractions;

[JsonSerializable(typeof(Message))]
[JsonSerializable(typeof(Status))]
[JsonSerializable(typeof(Version))]
[JsonSerializable(typeof(Information))]
internal sealed partial class AmethystJsonSerializerContext : JsonSerializerContext;