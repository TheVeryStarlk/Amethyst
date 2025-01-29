using System.Text.Json.Serialization;
using Amethyst.Components.Messages;

namespace Amethyst.Components;

[JsonSerializable(typeof(Message))]
internal partial class AmethystJsonSerializerContext : JsonSerializerContext;