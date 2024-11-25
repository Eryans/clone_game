using Godot;

public partial class Throwable : RigidBody3D
{
	private bool _isHolded = true;
	private float _wakeUpTimerDuration = 2.0f;
	private Node3D original;
	private Timer _wakeUpTimer = new();
	public override void _Ready()
	{
		AddChild(_wakeUpTimer);
		_wakeUpTimer.Timeout += OnWakeUpTimerTimeout;
	}

	public override void _PhysicsProcess(double delta)
	{
		if (LinearVelocity.Length() < 1 && !_isHolded)
		{
			_wakeUpTimer.Paused = false;
		}
		else
		{
			_wakeUpTimer.Paused = true;
		}
	}

	public void Setup(Node3D node)
	{
		original = node;
		AddChild(node.GetNode<Node3D>("Model").Duplicate());
	}

	public void Yeet(Vector3 yeetDirection)
	{
		ApplyCentralImpulse(yeetDirection);
		AddChild(original.GetNode<CollisionShape3D>("CollisionShape3D").Duplicate());
		_wakeUpTimer.Start(_wakeUpTimerDuration);
		_isHolded = false;
	}

	private void OnWakeUpTimerTimeout()
	{
		GetTree().CurrentScene.AddChild(original);
		original.GlobalPosition = GlobalPosition;
		QueueFree();
	}
}
