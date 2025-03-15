namespace Amethyst.Abstractions.Networking;

/// <summary>
/// Represents a <see cref="IOutgoingPacket"/> to the <see cref="IClient"/>.
/// </summary>
public interface IOutgoingPacket
{
    /// <summary>
    /// The <see cref="IOutgoingPacket"/> identifier.
    /// </summary>
    public int Identifier { get; }

    /// <summary>
    /// The length of the <see cref="IOutgoingPacket"/> in <see cref="byte"/>s.
    /// </summary>
    public int Length { get; }

    /// <summary>
    /// Writes the properties of the <see cref="IOutgoingPacket"/> packet.
    /// </summary>
    /// <param name="span">The <see cref="Span{T}"/> to write to.</param>
    public void Write(Span<byte> span);
}