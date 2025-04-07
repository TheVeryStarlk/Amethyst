using Amethyst.Abstractions.Entities.Player;

namespace Amethyst.Abstractions.Networking.Packets.Play;

public sealed class ChangeStatePacket(IState state) : IOutgoingPacket
{
    public int Identifier => 43;

    public IState State => state;
}

public interface IState
{
    public byte Identifier { get; }
}

public interface IValueState : IState
{
    public float Value { get; }
}

public sealed class StartRainingState : IState
{
    public byte Identifier => 1;
}

public sealed class StopRainingState : IState
{
    public byte Identifier => 2;
}

public sealed class GameModeState(GameMode gameMode) : IValueState
{
    public byte Identifier => 3;

    public float Value => (byte) gameMode;
}

public sealed class EnterCreditsState : IState
{
    public byte Identifier => 4;
}

public sealed class DemoState(Demo demo) : IValueState
{
    public byte Identifier => 5;

    public float Value => (byte) demo;
}

public enum Demo
{
    Welcome,
    MovementControls = 101,
    JumpControls = 102,
    InventoryControls = 103
}