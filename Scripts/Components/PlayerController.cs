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
	[Export]
	public CharacterBody3D ControlledEntity { get; private set; }
	[Export]
	public Grabber Grabber { get; private set; }

	public static event Action<bool> RightClick;
	public static event Action LeftClick;
	public override void _Ready()
	{
		RayCastCamera3d.MouseMoved += OnWorld3DMousePositionMovement;
	}
	public override void _PhysicsProcess(double delta)
	{
		Vector3 velocity = ControlledEntity.Velocity;
		if (!ControlledEntity.IsOnFloor())
		{
			velocity += ControlledEntity.GetGravity() * (float)delta;
		}

		if (Input.IsActionJustPressed("jump") && ControlledEntity.IsOnFloor())
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
			velocity.X = Mathf.MoveToward(ControlledEntity.Velocity.X, 0, Speed);
			velocity.Z = Mathf.MoveToward(ControlledEntity.Velocity.Z, 0, Speed);
		}

		ControlledEntity.Velocity = velocity;
		ControlledEntity.MoveAndSlide();

		HandleMouseInputs();
	}

	private void HandleMouseInputs()
	{
		if (Input.IsActionPressed("mouse_right"))
		{
			Grabber.Armed = true;
			Grabber.MakeClone();
		}
		if (Input.IsActionJustReleased("mouse_right"))
		{
			Grabber.Armed = false;
		}
		if (Input.IsActionJustPressed("mouse_left"))
		{
			Grabber.YEET();
		}
	}

	private void OnWorld3DMousePositionMovement(Vector3 mousePos)
	{
		ControlledEntity.Rotation = ControlledEntity.Rotation with { Y = Mathf.LerpAngle(ControlledEntity.Rotation.Y, Utils.LookAtTarget(ControlledEntity.GlobalPosition, mousePos), (float)GetPhysicsProcessDeltaTime() * _rotationSpeed) };
	}
}
