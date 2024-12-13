using Godot;
using System;
using System.Collections.Generic;

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
	public static event Action<Node3D> TargetChanged;
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
		if (Input.IsActionJustPressed("ui_accept"))
		{
			ChangeControlledEntity();
		}
	}

	private void ChangeControlledEntity()
	{
		var greenguyz = GetTree().GetNodesInGroup("greenguyz");
		if (greenguyz.Count > 1)
		{
			for (int i = 0; i < greenguyz.Count; i++)
			{
				if (greenguyz[i].GetInstanceId() == ControlledEntity.GetInstanceId())
				{
					ControlledEntity = (CharacterBody3D)greenguyz[i + 1 >= greenguyz.Count ? 0 : i + 1];
					TargetChanged?.Invoke(ControlledEntity);
					break;
				}
			}
		}
	}

	private void HandleMouseInputs()
	{
		if (Input.IsActionPressed("mouse_right"))
		{
			if (CloneManager.AvailableClone > 0)
			{
				Grabber.MakeClone();
				Grabber.ReadyToFire();
			}
			else
			{
				Grabber.Release();
			}
		}
		if (Input.IsActionJustReleased("mouse_right"))
		{
			Grabber.Release();
		}
		if (Input.IsActionJustPressed("mouse_left"))
		{
			if (CloneManager.AvailableClone > 0) Grabber.YEET();
		}
	}

	private void OnWorld3DMousePositionMovement(Vector3 mousePos)
	{
		ControlledEntity.Rotation = ControlledEntity.Rotation with { Y = Mathf.LerpAngle(ControlledEntity.Rotation.Y, Utils.LookAtTarget(ControlledEntity.GlobalPosition, mousePos), (float)GetPhysicsProcessDeltaTime() * _rotationSpeed) };
	}
}
