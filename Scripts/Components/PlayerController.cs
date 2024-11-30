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
	[Export]
	private float _rotationSpeed = 3f;
	private CharacterBody3D _parent;
	public override void _Ready()
	{
		_parent = GetParent<CharacterBody3D>();
		RayCastCamera3d.MouseMoved += OnWorld3DMousePositionMovement;
	}
	public override void _PhysicsProcess(double delta)
	{
		Vector3 velocity = _parent.Velocity;
		if (!_parent.IsOnFloor())
		{
			velocity += _parent.GetGravity() * (float)delta;
		}

		if (Input.IsActionJustPressed("jump") && _parent.IsOnFloor())
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
			velocity.X = Mathf.MoveToward(_parent.Velocity.X, 0, Speed);
			velocity.Z = Mathf.MoveToward(_parent.Velocity.Z, 0, Speed);
		}

		_parent.Velocity = velocity;
		_parent.MoveAndSlide();
	}

	private void OnWorld3DMousePositionMovement(Vector3 mousePos)
	{
		_parent.Rotation = _parent.Rotation with { Y = Mathf.LerpAngle(_parent.Rotation.Y, Utils.LookAtTarget(_parent.GlobalPosition, mousePos), (float)GetPhysicsProcessDeltaTime() * _rotationSpeed) };
	}
}
