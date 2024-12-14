using System;
using Godot;

public partial class GreenGuyThrowable : RigidBody3D
{
	private bool _isHolded = true;
	private float _wakeUpTimerDuration = 2.0f;
	private Timer _wakeUpTimer = new();
	private AudioStreamPlayer _yeetedSoundPlayer;
	private AudioStreamPlayer _bompSoundPlayer;

	public static event Action<GreenGuy> ThrownGreenGuyWakesUp;
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
		if (_isHolded)
		{
			LinearVelocity = Vector3.Zero;
		}
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
			if (IsInstanceValid(_bompSoundPlayer)) { _bompSoundPlayer.Play(); }
		}
	}


	public void Yeet(Vector3 yeetDirection)
	{
		ApplyCentralImpulse(yeetDirection);
		if (IsInstanceValid(_yeetedSoundPlayer)) _yeetedSoundPlayer.Play();
		_wakeUpTimer.Start(_wakeUpTimerDuration);
		_isHolded = false;
	}

	private void OnWakeUpTimerTimeout()
	{
		PackedScene greenGuyToSpawn = GD.Load<PackedScene>("res://Nodes/Entities/green_guy.tscn");
		GreenGuy newGreenGuy = (GreenGuy)greenGuyToSpawn.Instantiate();
		GetTree().CurrentScene.AddChild(newGreenGuy);
		newGreenGuy.GlobalPosition = GlobalPosition + Vector3.Up;
		ThrownGreenGuyWakesUp?.Invoke(newGreenGuy);
		QueueFree();
	}
}
