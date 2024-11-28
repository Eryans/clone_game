using Godot;
using System;

public partial class FollowTarget : NavigationAgent3D
{
	[Export]
	public AudioStreamPlayer WalkSoundPlayer;
	[Export]
	public float Speed = 5.0f;
	private CharacterBody3D characterBody3D;
	private Vector3 _target;
	public override void _Ready()
	{
		_target = Vector3.Zero;
		characterBody3D = GetParent<CharacterBody3D>();
	}

	public override void _PhysicsProcess(double delta)
	{
		TargetPosition = _target;
		if (NavigationServer3D.MapGetIterationId(GetNavigationMap()) == 0) return;
		if (IsNavigationFinished()) return;
		Vector3 nextPathPosition = GetNextPathPosition();
		Vector3 newVelocity = characterBody3D.GlobalPosition.DirectionTo(nextPathPosition) * Speed;
		if (DistanceToTarget() > 2.5)
		{
			characterBody3D.Velocity = newVelocity;
		}
		else
		{
			characterBody3D.Velocity = Vector3.Zero;
		}
		if (characterBody3D.Velocity.Length() > 0)
		{
			if (!WalkSoundPlayer.Playing) WalkSoundPlayer.Play();
		}
		else
		{
			WalkSoundPlayer.Stop();
		}
		characterBody3D.MoveAndSlide();
		LookAtTarget();

	}
	public void SetupTarget(Vector3 newTarget)
	{
		_target = newTarget;
	}

	private void LookAtTarget()
	{
		characterBody3D.LookAt(_target, Vector3.Up);
		Vector3 rotation = characterBody3D.Rotation;
		rotation.Z = 0;
		rotation.X = 0;
		characterBody3D.Rotation = rotation;
	}

}
