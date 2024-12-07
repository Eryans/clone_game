using Godot;
using System;

public partial class Dead : State
{
    [Export]
    private CharacterBody3D _parent;
    [Export]
    private NavigationAgent3D _navAgent;
    [Export]
    private EnemyAnimationTree _animationTree;
    public override void Enter()
    {
        GD.Print("aaaah dead");
        _parent.Velocity = Vector3.Zero;
        _animationTree.SetCurrentAnimation(EnemyAnimationTree.AnimationState.DEATH);
    }

    public override void PhysicsProcess(double delta)
    {
        _parent.Velocity += _parent.GetGravity();
        _parent.MoveAndSlide();
    }
}
