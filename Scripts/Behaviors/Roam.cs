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
    [Export(PropertyHint.Range, "1,10")]
    private float _maxSecondsBeforeTargetChange = 10f;
    private Timer _newTargetTimer = new();
    private RandomNumberGenerator rng = new();
    private Vector3 _startPosition;
    public override void _Ready()
    {
        AddChild(_newTargetTimer);
        _newTargetTimer.Timeout += OnNewTargetTimerTimeout;
        _startPosition = _parent.GlobalPosition;
    }
    public override void Enter()
    {
        _navAgent.TargetPosition = GetNewRoamTarget();
        StartTimer();
    }
    public override void Exit()
    {
        _newTargetTimer.Stop();
    }
    public override void PhysicsProcess(double delta)
    {
        RoamAround(delta);
    }

    private void RoamAround(double delta)
    {
        if (NavigationServer3D.MapGetIterationId(_navAgent.GetNavigationMap()) == 0) return;
        if (_navAgent.IsNavigationFinished()) return;


        Vector3 nextPathPosition = _navAgent.GetNextPathPosition();
        Vector3 newVelocity = _parent.GlobalPosition.DirectionTo(nextPathPosition) * _speed;
        _parent.Velocity = newVelocity;
        _parent.Rotation = _parent.Rotation with
        {
            Y = Mathf.LerpAngle(_parent.Rotation.Y,
            Utils.LookAtTarget(_parent.GlobalPosition, _navAgent.TargetPosition),
            (float)delta)
        };
        if (!_navAgent.IsTargetReached())
        {

            _parent.MoveAndSlide();
        }
    }

    private Vector3 GetNewRoamTarget()
    {
        Vector3 newTarget = new(rng.RandfRange(-_maxNewTargetDistance, _maxNewTargetDistance), 0, rng.RandfRange(-_maxNewTargetDistance, _maxNewTargetDistance));
        return _startPosition + newTarget;
    }
    private void OnNewTargetTimerTimeout()
    {
        SetNewTarget(GetNewRoamTarget());
    }

    private void StartTimer()
    {
        _newTargetTimer.Start(rng.RandfRange(5, _maxSecondsBeforeTargetChange));
    }


    private void SetNewTarget(Vector3 newTarget)
    {
        var rid = _navAgent.GetNavigationMap();
        _navAgent.TargetPosition = NavigationServer3D.MapGetClosestPoint(rid, newTarget);
        StartTimer();
    }
}
