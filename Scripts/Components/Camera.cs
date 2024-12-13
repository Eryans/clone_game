using Godot;
using System;

public partial class Camera : SpringArm3D
{
    [Export]
    public Node3D Target { get; private set; }
    [Export]
    public float LerpWeight { get; private set; }

    public override void _Ready()
    {
        PlayerController.TargetChanged += OnPlayerControllerTargetChange;
    }
    public override void _PhysicsProcess(double delta)
    {
        if (IsInstanceValid(Target))
            GlobalPosition = GlobalPosition.Lerp(Target.GlobalPosition, LerpWeight);
    }

    private void OnPlayerControllerTargetChange(Node3D newTarget)
    {
        Target = newTarget;
    }
}
