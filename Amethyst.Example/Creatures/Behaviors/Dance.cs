using Amethyst.Abstractions;
using Amethyst.Abstractions.Entities;
using Amethyst.Abstractions.Entities.Objects;
using Amethyst.Extensions;

namespace Amethyst.Example.Creatures.Behaviors;

/// <summary>
/// Swings the heads, arms and legs of armor stands to a drum beat, with a spin every few bars.
/// </summary>
internal sealed class Dance(Position stage, IArmorStand[] dancers) : IBehavior
{
    private readonly float[] facing = dancers.Select(dancer => dancer.Yaw).ToArray();

    private long age;

    public bool Step()
    {
        if (!dancers.Any(dancer => dancer.Tracked()))
        {
            return false;
        }

        age++;

        Beat();

        // Poses are metadata, so every other tick is plenty.
        if (age % 2 != 0)
        {
            return true;
        }

        for (var index = 0; index < dancers.Length; index++)
        {
            var dancer = dancers[index];

            var phase = age * 0.25 + index * 0.8;
            var swing = (float) Math.Sin(phase);
            var sway = (float) Math.Cos(phase);

            dancer.Head = new EulerAngle(sway * 15, swing * 25, 0);
            dancer.Body = new EulerAngle(0, swing * 12, 0);
            dancer.LeftArm = new EulerAngle(-100 - swing * 70, 0, -15 - sway * 25);
            dancer.RightArm = new EulerAngle(-100 + swing * 70, 0, 15 + sway * 25);
            dancer.LeftLeg = new EulerAngle(swing * 35, 0, -5);
            dancer.RightLeg = new EulerAngle(-swing * 35, 0, 5);

            dancer.Refresh();

            // Everyone spins around together every eight seconds.
            var bar = age % 160;

            if (bar < 40)
            {
                dancer.Look(facing[index] + bar * 9, 0);
            }
        }

        return true;
    }

    private void Beat()
    {
        var world = dancers[0].World;

        switch (age % 20)
        {
            case 0:
                Motion.Sound(world, Sounds.NoteBd, stage, 2F, 0.7F);
                break;

            case 10:
                Motion.Sound(world, Sounds.NoteSnare, stage, 2F, 1F);
                break;

            case 5 or 15:
                Motion.Sound(world, Sounds.NoteHat, stage, 1.5F, 1.4F);
                break;
        }
    }
}
