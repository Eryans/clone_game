using Godot;
using System;

public partial class Attack : State
{
    [Export]
    private EnemyAnimationTree _animationTree;
    [Export]
    private CharacterBody3D _parent;

    public override void Enter()
    {
        _parent.Velocity = Vector3.Zero;
        _animationTree.Set("parameters/AttackOneShot/request", (int)AnimationNodeOneShot.OneShotRequest.Fire);
        // ChangeToState("chase");
    }

    public override void Process(double delta)
    {
        if (!(bool)_animationTree.Get("parameters/AttackOneShot/active"))
        {
            ChangeToState("chase");
        }
    }
}
