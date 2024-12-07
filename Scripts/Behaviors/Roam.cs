using Godot;
using System;
using System.Runtime.CompilerServices;

public partial class Roam : State
{
    [Export]
    private NavigationAgent3D _navAgent;
    [Export]
    private CharacterBody3D _parent;
    [Export]
    private float _speed = 3.0f;
    [Export]
    private float _maxNewTargetDistance = 15f;
    [Export]
    private EnemyAnimationTree _animationTree;
    private RandomNumberGenerator rng = new();
    private Vector3 _startPosition;
    public override void _Ready()
    {
        _startPosition = _parent.GlobalPosition;
    }
    public override void Enter()
    {
        SetNewTarget(GetNewRoamTarget());
        _animationTree.SetCurrentAnimation(EnemyAnimationTree.AnimationState.WALK);

    }

    public override void PhysicsProcess(double delta)
    {
        RoamAround(delta);
    }

    private void RoamAround(double delta)
    {
        if (NavigationServer3D.MapGetIterationId(_navAgent.GetNavigationMap()) == 0) return;


        Vector3 nextPathPosition = _navAgent.GetNextPathPosition();
        Vector3 newVelocity = _parent.GlobalPosition.DirectionTo(nextPathPosition) * _speed;
        if (_navAgent.IsNavigationFinished())
        {
            _parent.Velocity = Vector3.Zero;
            ChangeToState("idle");
            return;
        }
        _parent.Velocity = newVelocity;
        _parent.Rotation = _parent.Rotation with
        {
            Y = Mathf.LerpAngle(_parent.Rotation.Y,
            Utils.LookAtTarget(_parent.GlobalPosition, _navAgent.TargetPosition),
            (float)delta)
        };
        _parent.MoveAndSlide();
    }

    private Vector3 GetNewRoamTarget()
    {
        Vector3 newTarget = new(rng.RandfRange(-_maxNewTargetDistance, _maxNewTargetDistance), 0, rng.RandfRange(-_maxNewTargetDistance, _maxNewTargetDistance));
        return _startPosition + newTarget;
    }



    private void SetNewTarget(Vector3 newTarget)
    {
        var rid = _navAgent.GetNavigationMap();
        _navAgent.TargetPosition = NavigationServer3D.MapGetClosestPoint(rid, newTarget);
    }
}
