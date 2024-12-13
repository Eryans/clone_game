using System.Collections.Generic;
using Godot;

[GlobalClass]
public partial class CloneManager : Node
{
    private static int MaxClone = 2;
    public static int AvailableClone { get; private set; }

    public override void _Ready()
    {
        AvailableClone = MaxClone;
    }

    public static void AddClone()
    {
        if (AvailableClone > 0) AvailableClone--;
    }
    public static void RemoveClone()
    {
        if (AvailableClone < MaxClone) AvailableClone++;
    }


}