using Godot;

public partial class Grabber : RayCast3D
{
	[ExportGroup("Yeet")]
	[Export]
	public AudioStreamPlayer YeetSound;
	[Export]
	private Vector3 yeetDirection = new(0, 15, -5);
	[Export]
	private int yeetPreviewStep = 20;
	[Export]
	private float yeetDirectionChangeAmount = .25f;
	[Export]
	private CharacterBody3D Target;
	private Marker3D grabbedObjectPoint;
	private Throwable currentHoldedObject;
	private PackedScene _greenGuy;
	private PackedScene _throwableScene;
	private Path3D yeetPreviewerPath;
	public bool Armed;
	public override void _Ready()
	{
		grabbedObjectPoint = GetNode<Marker3D>("Marker3D");
		_throwableScene = GD.Load<PackedScene>("res://Nodes/Components/throwable.tscn");
		_greenGuy = GD.Load<PackedScene>("res://Nodes/Entities/green_guy.tscn");
		yeetPreviewerPath = GetNode<Path3D>("Path3D");
		for (int i = 0; i < yeetPreviewStep; i++)
		{
			yeetPreviewerPath.Curve.AddPoint(Transform.Origin + Vector3.Up * i);
		}
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		if (@event is InputEventMouseButton mb && Armed)
		{
			if (mb.ButtonIndex == MouseButton.WheelUp)
			{
				if (Input.IsActionPressed("shift"))
				{
					yeetDirection.Y += yeetDirectionChangeAmount;
				}
				else
				{
					yeetDirection.Z -= yeetDirectionChangeAmount;
				}
				YeetPreview(GetPhysicsProcessDeltaTime());
			}
			else if (mb.ButtonIndex == MouseButton.WheelDown)
			{
				if (Input.IsActionPressed("shift"))
				{
					yeetDirection.Y -= yeetDirectionChangeAmount;
				}
				else
				{
					yeetDirection.Z += yeetDirectionChangeAmount;
				}
				YeetPreview(GetPhysicsProcessDeltaTime());
			}
		}
	}
	public override void _Process(double delta)
	{
		yeetDirection.Z = Mathf.Clamp(yeetDirection.Z, -10, -2);
		yeetDirection.Y = Mathf.Clamp(yeetDirection.Y, 2, 10);
	}
	public override void _PhysicsProcess(double delta)
	{
		GlobalTransform = Target.GlobalTransform;
		Visible = Armed;
		CheckForCollision();
		YeetPreview(delta);
		if (IsInstanceValid(currentHoldedObject))
		{
			currentHoldedObject.GlobalPosition = grabbedObjectPoint.GlobalPosition;
		}
	}

	public void YEET()
	{
		if (IsInstanceValid(currentHoldedObject))
		{
			currentHoldedObject.GlobalTransform = grabbedObjectPoint.GlobalTransform;
			{
				ThrowCurrentHoldedObject();
			}
		}
	}

	private void CheckForCollision()
	{
		if (IsColliding())
		{
			var collision = GetCollider();
			if (collision is CharacterBody3D cb && cb.IsInGroup("grabbable"))
			{
				if (Input.IsActionJustPressed("action") && !IsInstanceValid(currentHoldedObject))
				{
					SetCurrentHoldedObject((Node3D)cb.Duplicate(7));
					cb.QueueFree();
				}
			}

		}
	}

	public void MakeClone()
	{
		if (!IsInstanceValid(currentHoldedObject))
		{
			Node clone = _greenGuy.Instantiate();
			SetCurrentHoldedObject((Node3D)clone);
		}

	}
	private void ThrowCurrentHoldedObject()
	{
		if (IsInstanceValid(currentHoldedObject))
		{
			if (IsInstanceValid(YeetSound)) YeetSound.Play();
			currentHoldedObject.Yeet(Target.Transform.Basis * yeetDirection);
			currentHoldedObject = null;
		}
	}

	private void YeetPreview(double delta)
	{
		Vector3 startPosition = grabbedObjectPoint.Transform.Origin;
		Vector3 gravity = Target.GetGravity();

		float accumulatedTime = 0.0f;

		for (int i = 0; i < yeetPreviewStep; i++)
		{
			accumulatedTime += (float)delta * 4;

			Vector3 currentPosition = startPosition
									+ yeetDirection * accumulatedTime + 0.5f
									* gravity * accumulatedTime * accumulatedTime + Vector3.Down;

			yeetPreviewerPath.Curve.SetPointPosition(i, yeetPreviewerPath.ToLocal(currentPosition));
		}
	}

	private void SetCurrentHoldedObject(Node3D entity)
	{
		currentHoldedObject = (Throwable)_throwableScene.Instantiate();
		currentHoldedObject.SetCollisionMaskValue(1, false);
		currentHoldedObject.SetCollisionMaskValue(2, true);
		currentHoldedObject.SetCollisionLayerValue(1, false);
		currentHoldedObject.SetCollisionLayerValue(2, true);
		currentHoldedObject.Setup(entity);
		GetTree().CurrentScene.AddChild(currentHoldedObject);
	}

}
