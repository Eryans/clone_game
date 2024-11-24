using Godot;
using System;

public partial class FollowTarget : NavigationAgent3D
{
	[Export]
	public Node3D target;
	[Export]
	public float Speed = 5.0f;
	private CharacterBody3D characterBody3D;
	public override void _Ready()
	{
		characterBody3D = GetParent<CharacterBody3D>();
	}

	public override void _PhysicsProcess(double delta)
	{
		if (IsInstanceValid(target))
		{
			TargetPosition = target.GlobalPosition;
			if (NavigationServer3D.MapGetIterationId(GetNavigationMap()) == 0) return;
			if (IsNavigationFinished()) return;
			Vector3 nextPathPosition = GetNextPathPosition();
			Vector3 newVelocity = characterBody3D.GlobalPosition.DirectionTo(nextPathPosition) * Speed;
			OnVelocityComputed(newVelocity);
		}

	}

	public void OnVelocityComputed(Vector3 safeVelocity)
	{
		characterBody3D.Velocity = safeVelocity;
		characterBody3D.MoveAndSlide();
	}
}
