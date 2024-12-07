using Godot;
using System;

public partial class EnemyAI : Node
{
    [Export]
    private Node3D _target;
    [Export]
    private StateMachine stateMachine;
    [Export]
    private RayCast3D rayCast3D;
    [Export]
    private float _distanceToSeePlayer = 15f;
    [Export]
    private Health _health;
    private CharacterBody3D _parent;
    public override void _Ready()
    {
        base._Ready();
        _parent = GetParent<CharacterBody3D>();
        _target = GetTree().CurrentScene.GetNode<CharacterBody3D>("Player");
        _health.EntityDie += OnDeath;
        _health.EntityHit += OnHit;
    }

    public override void _PhysicsProcess(double delta)
    {
        if (_health.IsAlive())
        {
            TargetDetection(delta);
        }
    }
    private void TargetDetection(double delta)
    {
        if (_parent.GlobalPosition.DistanceTo(_target.GlobalPosition) < _distanceToSeePlayer)
        {
            Vector3 directionToTarget = _parent.GlobalPosition.DirectionTo(_target.GlobalPosition);
            rayCast3D.TargetPosition = rayCast3D.ToLocal(_target.GlobalPosition);
            if (rayCast3D.IsColliding())
            {
                var collider = rayCast3D.GetCollider();

                if (directionToTarget.Dot(-_parent.Basis.Z) > Mathf.Pi / 4 && collider is Player)
                {
                    stateMachine.ChangeState("chase");
                    Chase currentState = (Chase)stateMachine.CurrentState;
                    if (IsInstanceValid(currentState)) currentState.SetNewTarget(_target);
                }
            }
        }
    }
    private void OnHit()
    {
        stateMachine.ChangeState("hit");
    }
    private void OnDeath()
    {
        stateMachine.ChangeState("dead");
    }
}
