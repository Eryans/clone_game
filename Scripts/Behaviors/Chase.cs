using Godot;
using System;

public partial class Chase : State
{
    [Export]
    private NavigationAgent3D _navAgent;
    [Export]
    private Node3D _target;
    [Export]
    private CharacterBody3D _parent;
    [Export]
    private float speed = 3.0f;

    public override void Enter()
    {

    }
    public override void PhysicsProcess(double delta)
    {
        if (_parent.GlobalPosition.DistanceTo(_target.GlobalPosition) < 20)
        {
            ChaseTarget();
        }
        else
        {
            ChangeToState("Roam");
        }
    }

    private void ChaseTarget()
    {
        if (NavigationServer3D.MapGetIterationId(_navAgent.GetNavigationMap()) == 0) return;
        _navAgent.TargetPosition = _target.GlobalPosition;
        if (_navAgent.IsNavigationFinished()) return;
        Vector3 nextPathPosition = _navAgent.GetNextPathPosition();
        Vector3 newVelocity = _parent.GlobalPosition.DirectionTo(nextPathPosition) * speed;
        _parent.Velocity = newVelocity;
        _parent.Rotation = _parent.Rotation with { Y = Utils.LookAtTarget(_parent.GlobalPosition, _target.GlobalPosition) };
        _parent.MoveAndSlide();
    }
}
