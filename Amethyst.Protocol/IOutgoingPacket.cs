namespace Amethyst.Protocol;

public interface IOutgoingPacket
{
    public int Identifier { get; }
}

/// <summary>
/// Represents a writeable <see cref="IOutgoingPacket"/>.
/// </summary>
/// <remarks>
/// This is intended to be internally used in Amethyst.
/// </remarks>
public interface IWriteable
{
    /// <summary>
    /// The <see cref="IOutgoingPacket"/>'s length.
    /// </summary>
    public int Length { get; }

    /// <summary>
    /// Writes the <see cref="IOutgoingPacket"/> instance to a <see cref="Span{T}"/>.
    /// </summary>
    /// <param name="span">The <see cref="Span{T}"/> to write to.</param>
    public void Write(Span<byte> span);
}