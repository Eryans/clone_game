using Godot;
using System;

public partial class Grabber : RayCast3D
{
	[Export]
	public AudioStreamPlayer YeetSound;
	private Node3D parent;
	private Marker3D grabbedObjectPoint;

	private Throwable currentHoldedObject;
	private PackedScene _throwableScene;
	public override void _Ready()
	{
		parent = GetParent<Node3D>();
		grabbedObjectPoint = GetNode<Marker3D>("Marker3D");
		_throwableScene = GD.Load<PackedScene>("res://Nodes/Components/throwable.tscn");
	}

	public override void _PhysicsProcess(double delta)
	{
		if (IsInstanceValid(currentHoldedObject))
		{
			currentHoldedObject.GlobalPosition = grabbedObjectPoint.GlobalPosition;
			currentHoldedObject.Rotation = grabbedObjectPoint.GlobalRotation;
			if (Input.IsActionJustPressed("mouse_left"))
			{
				ThrowCurrentHoldedObject();
			}
		}
		CheckForCollision();
	}

	public bool IsHoldingObject()
	{
		return IsInstanceValid(currentHoldedObject);
	}
	private void CheckForCollision()
	{
		if (IsColliding())
		{
			var collision = GetCollider();
			if (collision is CharacterBody3D cb)
			{
				if (Input.IsActionJustPressed("action") && !IsInstanceValid(currentHoldedObject))
				{
					SetCurrentHoldedObject((Node3D)cb.Duplicate());
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
			currentHoldedObject.Yeet(parent.GlobalTransform.Basis * new Vector3(0, 5, -15));
			currentHoldedObject = null;
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
