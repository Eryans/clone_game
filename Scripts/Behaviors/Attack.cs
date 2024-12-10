using Godot;
using System;

public partial class Attack : State
{
    [Export]
    private EnemyAnimationTree _animationTree;
    [Export]
    private CharacterBody3D _parent;
    private CharacterBody3D _target;
    public override void Enter()
    {
        _target = GetTree().CurrentScene.GetNode<GreenGuy>("Player");
        _parent.Velocity = Vector3.Zero;
        _animationTree.Set("parameters/AttackOneShot/request", (int)AnimationNodeOneShot.OneShotRequest.Fire);
        // ChangeToState("chase");
    }
    public override void PhysicsProcess(double delta)
    {
        _parent.Rotation = _parent.Rotation with
        {
            Y = Mathf.LerpAngle(_parent.Rotation.Y,
                Utils.LookAtTarget(_parent.GlobalPosition, _target.GlobalPosition),
                (float)delta * 1.5f)
        };
    }
    public override void Process(double delta)
    {
        if (!(bool)_animationTree.Get("parameters/AttackOneShot/active"))
        {
            ChangeToState("chase");
        }
    }
}
