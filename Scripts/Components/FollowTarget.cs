using Godot;
using System;

public partial class FollowTarget : NavigationAgent3D
{
	[Export]
	public AudioStreamPlayer WalkSoundPlayer;
	[Export]
	public float Speed = 5.0f;
	[Export]
	private float _rotationSpeed = 3f;
	private CharacterBody3D _parent;
	private Vector3 _target;
	public override void _Ready()
	{
		_target = Vector3.Zero;
		_parent = GetParent<CharacterBody3D>();
	}

	public override void _PhysicsProcess(double delta)
	{
		TargetPosition = _target;
		if (NavigationServer3D.MapGetIterationId(GetNavigationMap()) == 0) return;
		if (IsNavigationFinished()) return;
		Vector3 nextPathPosition = GetNextPathPosition();
		Vector3 newVelocity = _parent.GlobalPosition.DirectionTo(nextPathPosition) * Speed;
		if (DistanceToTarget() > 2.5)
		{
			_parent.Velocity = newVelocity;
		}
		else
		{
			_parent.Velocity = Vector3.Zero;
		}
		if (_parent.Velocity.Length() > 0)
		{
			if (!WalkSoundPlayer.Playing) WalkSoundPlayer.Play();
		}
		else
		{
			WalkSoundPlayer.Stop();
		}
		_parent.MoveAndSlide();
		LookAtTarget();

	}
	public void SetupTarget(Vector3 newTarget)
	{
		_target = newTarget;
	}

	private void LookAtTarget()
	{
		_parent.Rotation = _parent.Rotation with { Y = Mathf.LerpAngle(_parent.Rotation.Y, Utils.LookAtTarget(_parent.GlobalPosition, _target), (float)GetPhysicsProcessDeltaTime() * _rotationSpeed) };
	}

}
