using Godot;

public partial class CloneYeeter : Node3D
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
	private Node3D Target;
	private Marker3D grabbedObjectPoint;
	private PackedScene greenGuyToThrow;
	private Path3D yeetPreviewerPath;
	private bool canThrow = true;
	public bool Armed;
	public override void _Ready()
	{
		Visible = false;
		grabbedObjectPoint = GetNode<Marker3D>("Marker3D");
		greenGuyToThrow = GD.Load<PackedScene>("res://Nodes/Components/GreenGuyThrowable.tscn");
		yeetPreviewerPath = GetNode<Path3D>("Path3D");
		PlayerController.TargetChanged += OnPlayerControllerTargetChange;
		GreenGuyThrowable.ThrownGreenGuyWakesUp += CanThrowAgain;
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
	public override void _PhysicsProcess(double delta)
	{
		yeetDirection.Z = Mathf.Clamp(yeetDirection.Z, -10, -2);
		yeetDirection.Y = Mathf.Clamp(yeetDirection.Y, 2, 10);
	}

	public void ReadyToFire()
	{
		GlobalTransform = Target.GlobalTransform;
		Visible = true;
		Armed = true;
		// CheckForCollision();
		YeetPreview(GetPhysicsProcessDeltaTime());
	}

	public void Release()
	{
		Armed = false;
		Visible = false;
	}

	private void CanThrowAgain(GreenGuy _greenGuy)
	{
		canThrow = true;
	}

	public void YEET()
	{
		if (canThrow)
		{
			canThrow = false;
			GreenGuyThrowable toThrow = (GreenGuyThrowable)greenGuyToThrow.Instantiate();
			GetTree().CurrentScene.AddChild(toThrow);
			toThrow.GlobalTransform = grabbedObjectPoint.GlobalTransform;
			if (IsInstanceValid(YeetSound)) YeetSound.Play();
			toThrow.Yeet(Target.Transform.Basis * yeetDirection);
		}
	}

	private void YeetPreview(double delta)
	{
		Vector3 startPosition = grabbedObjectPoint.Transform.Origin;
		float gravityForce = (float)ProjectSettings.GetSetting("physics/3d/default_gravity");
		Vector3 gravity = Vector3.Down * gravityForce;
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

	private void OnPlayerControllerTargetChange(Node3D newTarget)
	{
		Target = newTarget;
	}
}
