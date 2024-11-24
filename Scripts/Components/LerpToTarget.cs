using Godot;
using System;

public partial class LerpToTarget : Node
{
    [Export]
    public Node3D Target;
    [Export]
    public float LerpWeight = .5f;
    private Node3D parent;

    public override void _Ready()
    {
        parent = GetParent<Node3D>();
    }

    public override void _PhysicsProcess(double delta)
    {
        if (IsInstanceValid(Target))
        {

            parent.GlobalPosition = parent.GlobalPosition.Lerp(Target.GlobalPosition, LerpWeight);
        }
    }
}
