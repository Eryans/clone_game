using Godot;
using System;

[GlobalClass]
public partial class Hurtbox : Area3D
{
    [Export]
    private Health _health;

    public override void _Ready()
    {
        BodyEntered += OnBodyEntered;
    }

    public void OnBodyEntered(Node3D body)
    {
        if (body is GreenGuyThrowable)
        {
            _health.TakeDamage(5);
        }
    }
}
