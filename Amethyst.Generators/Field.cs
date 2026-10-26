using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Amethyst.Generators;

// Named after the span reader and writer methods, so each one maps to Read{Kind} and Write{Kind}.
internal enum Kind
{
    Byte,
    Boolean,
    Short,
    UnsignedShort,
    Integer,
    Long,
    Float,
    Double,
    VariableInteger,
    VariableString,
    Position,
    Guid
}

internal readonly struct Field(string name, Kind kind, bool array, string? cast) : IEquatable<Field>
{
    public string Name => name;

    public Kind Kind => kind;

    public bool IsArray => array;

    // The enum to cast from and to, if any.
    public string? Cast => cast;

    public static bool TryCreate(IParameterSymbol parameter, bool reading, out Field field)
    {
        var variable = parameter.GetAttributes().Any(attribute => attribute.AttributeClass is { Name: "VariableAttribute" } type
                                                                  && type.ContainingNamespace.ToDisplayString() == "Amethyst.Networking");

        var type = parameter.Type;
        var array = type is IArrayTypeSymbol { Rank: 1 };

        if (array)
        {
            type = ((IArrayTypeSymbol) type).ElementType;
        }

        var cast = type.TypeKind is TypeKind.Enum ? type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat) : null;
        var kind = cast is not null ? variable ? Kind.VariableInteger : Kind.Byte : GetKind(type, variable);

        var name = SyntaxFacts.GetKeywordKind(parameter.Name) is SyntaxKind.None ? parameter.Name : $"@{parameter.Name}";
        field = new Field(name, kind ?? default, array, cast);

        // The span reader has no fixed integer, GUID or array methods.
        return kind is not null
               && (!variable || kind is Kind.VariableInteger)
               && !(array && cast is not null)
               && !(reading && (array || kind is Kind.Integer or Kind.Guid));
    }

    private static Kind? GetKind(ITypeSymbol type, bool variable)
    {
        return type.SpecialType switch
        {
            SpecialType.System_Byte => Kind.Byte,
            SpecialType.System_Boolean => Kind.Boolean,
            SpecialType.System_Int16 => Kind.Short,
            SpecialType.System_UInt16 => Kind.UnsignedShort,
            SpecialType.System_Int32 => variable ? Kind.VariableInteger : Kind.Integer,
            SpecialType.System_Int64 => Kind.Long,
            SpecialType.System_Single => Kind.Float,
            SpecialType.System_Double => Kind.Double,
            SpecialType.System_String => Kind.VariableString,
            _ => type.ToDisplayString() switch
            {
                "Amethyst.Abstractions.Entities.Position" => Kind.Position,
                "System.Guid" => Kind.Guid,
                _ => null
            }
        };
    }

    public bool Equals(Field other)
    {
        return name == other.Name && kind == other.Kind && array == other.IsArray && cast == other.Cast;
    }

    public override bool Equals(object? other)
    {
        return other is Field field && Equals(field);
    }

    public override int GetHashCode()
    {
        return (name, kind, array, cast).GetHashCode();
    }
}