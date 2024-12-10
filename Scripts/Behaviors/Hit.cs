using Godot;
using System;

public partial class Hit : State
{
    [Export]
    private EnemyAnimationTree _animationTree;
    [Export]
    private CharacterBody3D _parent;

    public override void Enter()
    {
        _parent.Velocity = Vector3.Zero;
        _animationTree.Set("parameters/HitOneShot/request", (int)AnimationNodeOneShot.OneShotRequest.Fire);
    }

    public override void Process(double delta)
    {
        if (!(bool)_animationTree.Get("parameters/HitOneShot/active"))
        {
            ChangeToState("chase");
        }
    }
}
