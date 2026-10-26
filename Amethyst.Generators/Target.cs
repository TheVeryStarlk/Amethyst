using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Amethyst.Generators;

[Flags]
internal enum Members
{
    None = 0,
    Create = 1,
    Length = 2,
    Write = 4
}

// An ingoing packet or a serializer, along with the members left for the generator.
internal sealed class Target(
    string type,
    string? packet,
    string? space,
    string name,
    Members members,
    EquatableArray<Field> fields,
    EquatableArray<Problem> problems) : IEquatable<Target>
{
    public string Type => type;

    // The outgoing packet a serializer is for.
    public string? Packet => packet;

    public string? Namespace => space;

    public string Name => name;

    public Members Members => members;

    public EquatableArray<Field> Fields => fields;

    public EquatableArray<Problem> Problems => problems;

    public bool Equals(Target? other)
    {
        return other is not null
               && type == other.Type
               && packet == other.Packet
               && space == other.Namespace
               && name == other.Name
               && members == other.Members
               && fields.Equals(other.Fields)
               && problems.Equals(other.Problems);
    }

    public override bool Equals(object? other)
    {
        return Equals(other as Target);
    }

    public override int GetHashCode()
    {
        return (type, packet, space, name, members, fields, problems).GetHashCode();
    }
}

// Keeps the location as plain values, holding on to syntax trees would defeat the caching.
internal readonly struct Problem(string parameter, string type, bool reading, string path, TextSpan span, LinePositionSpan line) : IEquatable<Problem>
{
    public static DiagnosticDescriptor Descriptor { get; } = new(
        "AMETHYST001",
        "Unsupported packet field",
        "The parameter '{0}' of type '{1}' cannot be {2}",
        "Amethyst.Generators",
        DiagnosticSeverity.Error,
        true,
        "Fields can be primitives, strings, positions, GUIDs, enums or arrays of those. [Variable] only applies to integers and enums. Fixed integers, GUIDs and arrays cannot be read.");

    public static Problem Create(IParameterSymbol parameter, bool reading)
    {
        var location = parameter.Locations[0];

        return new Problem(
            parameter.Name,
            parameter.Type.ToDisplayString(),
            reading,
            location.SourceTree?.FilePath ?? string.Empty,
            location.SourceSpan,
            location.GetLineSpan().Span);
    }

    public Diagnostic ToDiagnostic()
    {
        return Diagnostic.Create(Descriptor, Location.Create(path, span, line), parameter, type, reading ? "read" : "written");
    }

    private (string, string, bool, string, TextSpan, LinePositionSpan) Key => (parameter, type, reading, path, span, line);

    public bool Equals(Problem other)
    {
        return Key.Equals(other.Key);
    }

    public override bool Equals(object? other)
    {
        return other is Problem problem && Equals(problem);
    }

    public override int GetHashCode()
    {
        return Key.GetHashCode();
    }
}