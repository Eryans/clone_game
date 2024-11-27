using Godot;
using System;

public partial class PlayerController : Node
{
	[Export]
	public float Speed = 5.0f;
	[Export]
	public float JumpVelocity = 4.5f;
	[Export]
	public AudioStreamPlayer WalkSoundStreamPlayer;
	private CharacterBody3D characterBody3D;
	public override void _Ready()
	{
		characterBody3D = GetParent<CharacterBody3D>();
		GlobalSignals.Instance.World3DMousePosition += OnWorld3DMousePositionMovement;
	}
	public override void _PhysicsProcess(double delta)
	{
		Vector3 velocity = characterBody3D.Velocity;
		if (!characterBody3D.IsOnFloor())
		{
			velocity += characterBody3D.GetGravity() * (float)delta;
		}

		if (Input.IsActionJustPressed("jump") && characterBody3D.IsOnFloor())
		{
			velocity.Y = JumpVelocity;
		}

		Vector2 inputDir = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
		Vector3 direction = new Vector3(inputDir.X, 0, inputDir.Y).Normalized();
		if (direction != Vector3.Zero)
		{
			velocity.X = direction.X * Speed;
			velocity.Z = direction.Z * Speed;
			if (IsInstanceValid(WalkSoundStreamPlayer) && !WalkSoundStreamPlayer.Playing) WalkSoundStreamPlayer.Play();
		}
		else
		{
			if (IsInstanceValid(WalkSoundStreamPlayer)) WalkSoundStreamPlayer.Stop();
			velocity.X = Mathf.MoveToward(characterBody3D.Velocity.X, 0, Speed);
			velocity.Z = Mathf.MoveToward(characterBody3D.Velocity.Z, 0, Speed);
		}

		characterBody3D.Velocity = velocity;
		characterBody3D.MoveAndSlide();
	}

	private void OnWorld3DMousePositionMovement(Vector3 mousePos)
	{
		Vector3 rotation = characterBody3D.Rotation;
		Vector2 direction = -new Vector2(mousePos.X - characterBody3D.GlobalPosition.X, mousePos.Z - characterBody3D.GlobalPosition.Z);
		rotation.Y = Mathf.Atan2(direction.X, direction.Y);
		characterBody3D.Rotation = rotation;
	}
}
