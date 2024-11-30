using Godot;
using System;

public partial class Dead : State
{
    [Export]
    private CharacterBody3D _parent;
    [Export]
    private NavigationAgent3D _navAgent;
    public override void Enter()
    {
        _parent.Velocity = Vector3.Zero;
        GD.Print("ARGL I AM DEAD");
    }

    public override void PhysicsProcess(double delta)
    {
        _parent.Velocity += _parent.GetGravity();
        _parent.Rotation = _parent.Rotation with { X = Mathf.LerpAngle(_parent.Rotation.X, Mathf.Pi / 2, (float)delta * 3) };
        _parent.MoveAndSlide();
    }
}
