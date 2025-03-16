namespace Amethyst.Networking.Packets;

internal interface IProcessor
{
    public void Process(Client client);
}