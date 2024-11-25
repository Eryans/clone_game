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
		characterBody3D.LookAt(mousePos, Vector3.Up);
		Vector3 rotation = characterBody3D.Rotation;
		rotation.Z = 0;
		rotation.X = 0;
		characterBody3D.Rotation = rotation;
	}
}
