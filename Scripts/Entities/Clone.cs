using Godot;
using System;

public partial class Clone : CharacterBody3D
{
	[Export]
	public Node3D Target;

	private FollowTarget followTarget;
	private GreenGuy player;
	public override void _Ready()
	{
		followTarget = GetNode<FollowTarget>("FollowTarget");
		player = GetTree().CurrentScene.GetNode<GreenGuy>("Player");
	}
	public override void _Process(double delta)
	{
		if (Target == null)
		{
			followTarget.SetupTarget(player.GlobalPosition);
		}
		else
		{
			followTarget.SetupTarget(Target.GlobalPosition);
		}
	}

}
