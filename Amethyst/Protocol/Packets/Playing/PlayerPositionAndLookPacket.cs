﻿using Amethyst.Api.Components;

namespace Amethyst.Protocol.Packets.Playing;

internal sealed class PlayerPositionAndLookPacket : IIngoingPacket<PlayerPositionAndLookPacket>, IOutgoingPacket
{
    static int IIngoingPacket<PlayerPositionAndLookPacket>.Identifier => 0x06;

    public int Identifier => 0x08;

    public required VectorF Vector { get; init; }

    public float Yaw { get; init; }

    public float Pitch { get; init; }

    public bool OnGround { get; init; }

    public static PlayerPositionAndLookPacket Read(MemoryReader reader)
    {
        return new PlayerPositionAndLookPacket
        {
            Vector = new VectorF(
                reader.ReadDouble(),
                reader.ReadDouble(),
                reader.ReadDouble()),

            Yaw = reader.ReadFloat(),
            Pitch = reader.ReadFloat(),
            OnGround = reader.ReadBoolean()
        };
    }

    public void Write(ref MemoryWriter writer)
    {
        writer.WriteDouble(Vector.X);
        writer.WriteDouble(Vector.Y);
        writer.WriteDouble(Vector.Z);
        writer.WriteFloat(Yaw);
        writer.WriteFloat(Pitch);
        writer.WriteByte(0);
    }

    public Task HandleAsync(Client client)
    {
        client.Player!.Vector = Vector;
        client.Player.Yaw = Yaw;
        client.Player.Pitch = Pitch;
        client.Player.OnGround = OnGround;
        return Task.CompletedTask;
    }
}