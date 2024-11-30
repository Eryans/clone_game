using Godot;
using System;

public partial class EnemyAI : Node
{
    [Export]
    private Vector3 _target;
    [Export]
    private float speed = 3.0f;
    [Export]
    private NavigationAgent3D _navAgent;
    private CharacterBody3D _parent;
    public override void _Ready()
    {
        base._Ready();
        _parent = GetParent<CharacterBody3D>();
    }

    public override void _PhysicsProcess(double delta)
    {



    }

}
