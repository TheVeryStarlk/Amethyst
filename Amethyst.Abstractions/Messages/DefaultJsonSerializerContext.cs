using System.Text.Json.Serialization;

namespace Playground.Abstractions.Messages;

[JsonSerializable(typeof(Message))]
[JsonSerializable(typeof(Status))]
[JsonSerializable(typeof(Version))]
[JsonSerializable(typeof(Information))]
internal sealed partial class DefaultJsonSerializerContext : JsonSerializerContext;