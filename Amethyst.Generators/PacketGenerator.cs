using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Amethyst.Generators;

[Generator]
public sealed class PacketGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var ingoing = context.SyntaxProvider
            .CreateSyntaxProvider(
                static (node, _) => Implements(node, "IIngoingPacket"),
                static (context, token) => GetTarget(context, "IIngoingPacket", "Amethyst.Networking.Packets", true, token))
            .Where(static target => target is not null)
            .Select(static (target, _) => target!);

        var serializers = context.SyntaxProvider
            .CreateSyntaxProvider(
                static (node, _) => Implements(node, "ISerializer"),
                static (context, token) => GetTarget(context, "ISerializer", "Amethyst.Networking.Serializers", false, token))
            .Where(static target => target is not null)
            .Select(static (target, _) => target!);

        var packets = ingoing
            .Select(static (target, _) => target.Type)
            .Collect();

        var pairs = serializers
            .Select(static (target, _) => (Packet: target.Packet!, Serializer: target.Type))
            .Collect();

        context.RegisterSourceOutput(packets, static (context, packets) => context.AddSource("PacketExtensions.g.cs", Source.WriteCreators(packets)));
        context.RegisterSourceOutput(pairs, static (context, pairs) => context.AddSource("OutgoingPacketExtensions.g.cs", Source.WriteSerializers(pairs)));

        context.RegisterSourceOutput(ingoing, static (context, target) => AddMembers(context, target));
        context.RegisterSourceOutput(serializers, static (context, target) => AddMembers(context, target));
    }

    // Only a syntactic check, so the semantic model is never touched for most classes.
    private static bool Implements(SyntaxNode node, string name)
    {
        return node is ClassDeclarationSyntax { BaseList: { } list } && list.Types.Any(type => GetName(type.Type) == name);
    }

    private static string? GetName(TypeSyntax type)
    {
        return type switch
        {
            QualifiedNameSyntax qualified => GetName(qualified.Right),
            GenericNameSyntax generic => generic.Identifier.ValueText,
            _ => null
        };
    }

    private static Target? GetTarget(GeneratorSyntaxContext context, string name, string space, bool reading, CancellationToken token)
    {
        var declaration = (ClassDeclarationSyntax) context.Node;
        var symbol = context.SemanticModel.GetDeclaredSymbol(declaration, token);

        // Partial classes are found once per declaration, so only the first one is kept.
        if (symbol is null || symbol.IsAbstract || symbol.DeclaringSyntaxReferences[0].GetSyntax(token) != declaration)
        {
            return null;
        }

        var contract = symbol.Interfaces.FirstOrDefault(type => type.Name == name
                                                                && type.IsGenericType
                                                                && type.ContainingNamespace.ToDisplayString() == space);

        if (contract is null)
        {
            return null;
        }

        var members = Members.None;

        // Only partial classes are generated, everything they already declare is left alone.
        if (declaration.Modifiers.Any(SyntaxKind.PartialKeyword) && symbol.ContainingType is null)
        {
            members = reading
                ? GetMissing(symbol, "Create", Members.Create)
                : GetMissing(symbol, "Length", Members.Length) | GetMissing(symbol, "Write", Members.Write);
        }

        var fields = ImmutableArray.CreateBuilder<Field>();
        var problems = ImmutableArray.CreateBuilder<Problem>();

        if (members is not Members.None)
        {
            foreach (var parameter in GetParameters(symbol, token))
            {
                if (Field.TryCreate(parameter, reading, out var field))
                {
                    fields.Add(field);
                }
                else
                {
                    problems.Add(Problem.Create(parameter, reading));
                }
            }
        }

        return new Target(
            symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
            reading ? null : contract.TypeArguments[0].ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
            symbol.ContainingNamespace.IsGlobalNamespace ? null : symbol.ContainingNamespace.ToDisplayString(),
            symbol.Name,
            members,
            new EquatableArray<Field>(fields.ToImmutable()),
            new EquatableArray<Problem>(problems.ToImmutable()));
    }

    private static Members GetMissing(INamedTypeSymbol symbol, string name, Members member)
    {
        return symbol.GetMembers(name).IsEmpty ? member : Members.None;
    }

    // The primary constructor declares the fields in the order they are sent.
    private static ImmutableArray<IParameterSymbol> GetParameters(INamedTypeSymbol symbol, CancellationToken token)
    {
        var constructor = symbol.InstanceConstructors.FirstOrDefault(constructor => constructor.DeclaringSyntaxReferences
            .Any(reference => reference.GetSyntax(token) is TypeDeclarationSyntax));

        return constructor?.Parameters ?? ImmutableArray<IParameterSymbol>.Empty;
    }

    private static void AddMembers(SourceProductionContext context, Target target)
    {
        foreach (var problem in target.Problems)
        {
            context.ReportDiagnostic(problem.ToDiagnostic());
        }

        if (target.Members is Members.None || !target.Problems.IsEmpty)
        {
            return;
        }

        context.AddSource($"{target.Type.Replace("global::", string.Empty)}.g.cs", Source.WriteMembers(target));
    }
}