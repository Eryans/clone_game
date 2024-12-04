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
	private CharacterBody3D parent;
	private Marker3D grabbedObjectPoint;
	private Throwable currentHoldedObject;
	private PackedScene _throwableScene;
	private Path3D yeetPreviewerPath;
	public bool IsHoldingObject { get => IsInstanceValid(currentHoldedObject); }
	public override void _Ready()
	{
		parent = GetParent<CharacterBody3D>();
		grabbedObjectPoint = GetNode<Marker3D>("Marker3D");
		_throwableScene = GD.Load<PackedScene>("res://Nodes/Components/throwable.tscn");
		yeetPreviewerPath = GetNode<Path3D>("Path3D");
		for (int i = 0; i < yeetPreviewStep; i++)
		{
			yeetPreviewerPath.Curve.AddPoint(Transform.Origin + Vector3.Up * i);
		}
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		if (@event is InputEventMouseButton mb && IsHoldingObject)
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
		if (IsInstanceValid(currentHoldedObject))
		{
			currentHoldedObject.GlobalTransform = grabbedObjectPoint.GlobalTransform;
			if (Input.IsActionJustPressed("mouse_left"))
			{
				ThrowCurrentHoldedObject();
			}
		}
		CheckForCollision();
		if (Input.IsActionJustPressed("action"))
		{
			YeetPreview(GetPhysicsProcessDeltaTime());
		}
		yeetPreviewerPath.Visible = IsHoldingObject;
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
	private void ThrowCurrentHoldedObject()
	{
		if (IsInstanceValid(currentHoldedObject))
		{
			if (IsInstanceValid(YeetSound)) YeetSound.Play();
			currentHoldedObject.Yeet(parent.Transform.Basis * yeetDirection);
			currentHoldedObject = null;
		}
	}

	private void YeetPreview(double delta)
	{
		Vector3 startPosition = grabbedObjectPoint.Transform.Origin;
		Vector3 gravity = parent.GetGravity();

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
		currentHoldedObject.GlobalPosition = grabbedObjectPoint.GlobalPosition;
	}

}
