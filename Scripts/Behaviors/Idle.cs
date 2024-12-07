using Godot;
using System;

public partial class Idle : State
{
    [Export(PropertyHint.Range, "1,10")]
    private float _maxSecondesBeforeRoamState = 10f;
    [Export]
    private EnemyAnimationTree _animationTree;
    [Export]
    private CharacterBody3D _parent;
    private Timer _changeToRoamStateTimer = new();
    private RandomNumberGenerator rng = new();

    public override void _Ready()
    {
        AddChild(_changeToRoamStateTimer);
        _changeToRoamStateTimer.Timeout += changeToRoamStateTimeout;
    }
    public override void Enter()
    {
        StartTimer();
        _parent.Velocity = Vector3.Zero;
        _animationTree.SetCurrentAnimation(EnemyAnimationTree.AnimationState.IDLE);
    }
    public override void Exit()
    {
        _changeToRoamStateTimer.Stop();
    }
    public override void _PhysicsProcess(double delta)
    {
        _parent.Velocity += _parent.GetGravity();
        _parent.MoveAndSlide();
    }
    private void changeToRoamStateTimeout()
    {
        ChangeToState("roam");
    }

    private void StartTimer()
    {
        _changeToRoamStateTimer.Start(rng.RandfRange(3, _maxSecondesBeforeRoamState));
    }
}
