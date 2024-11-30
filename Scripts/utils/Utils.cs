using Godot;
using System;

public partial class Utils : Node
{
    public static float LookAtTarget(Vector3 from, Vector3 to)
    {
        Vector2 direction = new(from.X - to.X, from.Z - to.Z);
        return Mathf.Atan2(direction.X, direction.Y);
    }
}
