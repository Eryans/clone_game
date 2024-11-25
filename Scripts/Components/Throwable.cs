using Godot;

public partial class Throwable : RigidBody3D
{
	private bool _isHolded = true;
	private float _wakeUpTimerDuration = 2.0f;
	private Node3D _original;
	private Timer _wakeUpTimer = new();
	private AudioStreamPlayer _yeetedSoundPlayer;
	private AudioStreamPlayer _bompSoundPlayer;
	public override void _Ready()
	{
		AddChild(_wakeUpTimer);
		_wakeUpTimer.Timeout += OnWakeUpTimerTimeout;
		_yeetedSoundPlayer = GetNode<AudioStreamPlayer>("YeetedSoundPlayer");
		_bompSoundPlayer = GetNode<AudioStreamPlayer>("BompSoundPlayer");
		ContactMonitor = true;
		MaxContactsReported = 10;
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
		foreach (var node in GetCollidingBodies())
		{
			if (node is CsgBox3D)
			{
				if (IsInstanceValid(_bompSoundPlayer)) { _bompSoundPlayer.Play(); }
			}
		}
	}

	public void Setup(Node3D node)
	{
		_original = node;
		AddChild(node.GetNode<Node3D>("Model").Duplicate());
	}

	public void Yeet(Vector3 yeetDirection)
	{
		ApplyCentralImpulse(yeetDirection);
		if (IsInstanceValid(_yeetedSoundPlayer)) _yeetedSoundPlayer.Play();
		AddChild(_original.GetNode<CollisionShape3D>("CollisionShape3D").Duplicate());
		_wakeUpTimer.Start(_wakeUpTimerDuration);
		_isHolded = false;
	}

	private void OnWakeUpTimerTimeout()
	{
		GetTree().CurrentScene.AddChild(_original);
		_original.GlobalPosition = GlobalPosition;
		QueueFree();
	}
}
