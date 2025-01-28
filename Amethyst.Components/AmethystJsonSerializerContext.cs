using System.Text.Json.Serialization;
using Amethyst.Components.Messages;

namespace Amethyst.Components;

[JsonSerializable(typeof(Message))]
public partial class AmethystJsonSerializerContext : JsonSerializerContext;