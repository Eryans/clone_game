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
	public Node3D ControlledEntity { get; private set; }
	[Export]
	public CloneYeeter CloneYeeter { get; private set; }
	[Export]
	public int MaxClones { get; private set; } = 2;

	private Node3D _lastControlledEntity;

	public static event Action<Node3D> TargetChanged;
	public override void _Ready()
	{
		RayCastCamera3d.MouseMoved += OnWorld3DMousePositionMovement;
		GreenGuyThrowable.ThrownGreenGuyWakesUp += OnGreenGuyThrowableWakesup;
	}
	public override void _PhysicsProcess(double delta)
	{
		if (ControlledEntity is CharacterBody3D controlledEntity)
		{

			Vector3 velocity = controlledEntity.Velocity;
			if (!controlledEntity.IsOnFloor())
			{
				velocity += controlledEntity.GetGravity() * (float)delta;
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
				velocity.X = Mathf.MoveToward(controlledEntity.Velocity.X, 0, Speed);
				velocity.Z = Mathf.MoveToward(controlledEntity.Velocity.Z, 0, Speed);
			}

			controlledEntity.Velocity = velocity;
			controlledEntity.MoveAndSlide();

			HandleMouseInputs();
			if (Input.IsActionJustPressed("ui_accept"))
			{
				ChangeControlledEntity();
			}
			if (Input.IsActionJustPressed("kill"))
			{
				KillControlledEntity();
			}
		}
	}
	private Godot.Collections.Array<Node> GetClones()
	{
		return GetTree().GetNodesInGroup("greenguyz");
	}
	private void ChangeControlledEntity()
	{
		var greenguyz = GetClones();
		if (greenguyz.Count > 1)
		{
			for (int i = 0; i < greenguyz.Count; i++)
			{
				if (greenguyz[i].GetInstanceId() == ControlledEntity.GetInstanceId())
				{
					_lastControlledEntity = (Node3D)greenguyz[i];
					ControlledEntity = (Node3D)greenguyz[i + 1 >= greenguyz.Count ? 0 : i + 1];
					TargetChanged?.Invoke(ControlledEntity);
					break;
				}
			}
		}
	}

	private void KillControlledEntity()
	{
		if (GetClones().Count > 1)
		{
			ChangeControlledEntity();
			_lastControlledEntity.QueueFree();
		}
	}
	private void OnGreenGuyThrowableWakesup(GreenGuy greenGuy)
	{
		if (ControlledEntity is GreenGuyThrowable)
		{
			ControlledEntity = greenGuy;
			TargetChanged?.Invoke(ControlledEntity);
		}
	}
	private void HandleMouseInputs()
	{
		var greenguyz = GetClones();
		if (Input.IsActionPressed("mouse_right"))
		{
			CloneYeeter.ReadyToFire();
			if (Input.IsActionJustPressed("mouse_left"))
			{
				if (greenguyz.Count > 0 && greenguyz.Count < MaxClones) CloneYeeter.YEET();
			}
		}
		if (Input.IsActionJustReleased("mouse_right"))
		{
			CloneYeeter.Release();
		}
	}

	private void OnWorld3DMousePositionMovement(Vector3 mousePos)
	{
		if (ControlledEntity is GreenGuy)
			ControlledEntity.Rotation = ControlledEntity.Rotation with { Y = Mathf.LerpAngle(ControlledEntity.Rotation.Y, Utils.LookAtTarget(ControlledEntity.GlobalPosition, mousePos), (float)GetPhysicsProcessDeltaTime() * _rotationSpeed) };
	}
}
