using Godot;
using System;

[GlobalClass]
public partial class Hitbox : Area3D
{
    [Export]
    private Health _health;

    public override void _Ready()
    {
        BodyEntered += OnBodyEntered;
    }

    public void OnBodyEntered(Node3D body)
    {
        if (body is Throwable)
        {
            _health.TakeDamage(5);
        }
    }
}
