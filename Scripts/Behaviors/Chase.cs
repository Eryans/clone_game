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
    [Export]
    private EnemyAnimationTree _animationTree;
    [Export]
    public float AttackDistance { get; private set; } = 4f;


    public override void Enter()
    {
        if (!IsInstanceValid(_target))
        {
            _target = GetTree().CurrentScene.GetNode<GreenGuy>("Player");
        }
        _animationTree.SetCurrentAnimation(EnemyAnimationTree.AnimationState.WALK);

    }
    public override void PhysicsProcess(double delta)
    {
        if (_parent.GlobalPosition.DistanceTo(_target.GlobalPosition) < AttackDistance)
        {
            ChangeToState("attack");
            return;
        }
        else if (_parent.GlobalPosition.DistanceTo(_target.GlobalPosition) < 20)
        {
            ChaseTarget(delta);
        }
        else
        {
            ChangeToState("Roam");
            return;
        }
    }
    public override void Exit()
    {
    }
    public void SetNewTarget(Node3D newTarget)
    {
        _target = newTarget;
    }
    private void ChaseTarget(double delta)
    {
        if (NavigationServer3D.MapGetIterationId(_navAgent.GetNavigationMap()) == 0) return;
        _navAgent.TargetPosition = _target.GlobalPosition;
        if (_navAgent.IsNavigationFinished()) return;
        Vector3 nextPathPosition = _navAgent.GetNextPathPosition();
        Vector3 newVelocity = _parent.GlobalPosition.DirectionTo(nextPathPosition) * speed;
        _parent.Velocity = newVelocity;
        _parent.Rotation = _parent.Rotation with
        {
            Y = Mathf.LerpAngle(_parent.Rotation.Y,
           Utils.LookAtTarget(_parent.GlobalPosition, _navAgent.TargetPosition),
           (float)delta)
        };
        _parent.MoveAndSlide();
    }
}
