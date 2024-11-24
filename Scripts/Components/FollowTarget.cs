using Godot;
using System;

public partial class FollowTarget : NavigationAgent3D
{
	private Vector3 _target;
	[Export]
	public float Speed = 5.0f;
	private CharacterBody3D characterBody3D;
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
		characterBody3D.Velocity = newVelocity;
		characterBody3D.MoveAndSlide();

	}
	public void SetupTarget(Vector3 newTarget)
	{
		_target = newTarget;
	}


}
