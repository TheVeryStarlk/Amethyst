using System.Text.Json.Serialization;
using Amethyst.Components.Messages;

namespace Amethyst.Components;

[JsonSerializable(typeof(Message))]
[JsonSerializable(typeof(Status))]
[JsonSerializable(typeof(Version))]
[JsonSerializable(typeof(Information))]
internal partial class AmethystJsonSerializerContext : JsonSerializerContext;