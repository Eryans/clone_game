using Godot;
using System;

public partial class Grabber : RayCast3D
{
	private Node3D parent;
	private Marker3D grabbedObjectPoint;

	private RigidBody3D currentHoldedObject;
	private bool holding = false;
	public override void _Ready()
	{
		parent = GetParent<Node3D>();
		grabbedObjectPoint = GetNode<Marker3D>("Marker3D");
	}

	public override void _PhysicsProcess(double delta)
	{
		if (IsInstanceValid(currentHoldedObject))
		{
			if (holding)
				currentHoldedObject.GlobalPosition = grabbedObjectPoint.GlobalPosition;
			if (Input.IsActionJustPressed("action"))
			{
				ThrowCurrentHoldedObject();
			}
		}
		CheckForCollision();
	}

	private void CheckForCollision()
	{
		if (IsColliding())
		{
			var collision = GetCollider();
			if (collision is CharacterBody3D cb)
			{
				if (Input.IsActionJustPressed("action"))
				{
					SetCurrentHoldedObject((MeshInstance3D)cb.GetNode<MeshInstance3D>("MeshInstance3D").Duplicate(),
					(CollisionShape3D)cb.GetNode<CollisionShape3D>("CollisionShape3D").Duplicate());
					cb.QueueFree();
				}
			}

		}
	}
	private void ThrowCurrentHoldedObject()
	{
		if (IsInstanceValid(currentHoldedObject))
		{
			holding = false;
			currentHoldedObject.ApplyCentralImpulse(parent.GlobalTransform.Basis * new Vector3(0, 1, 25));
			currentHoldedObject = null;
		}
	}

	private void SetCurrentHoldedObject(MeshInstance3D mesh, CollisionShape3D collisionShape)
	{
		currentHoldedObject = new RigidBody3D();
		currentHoldedObject.AddChild(mesh);
		currentHoldedObject.AddChild(collisionShape);
		currentHoldedObject.GlobalPosition = grabbedObjectPoint.GlobalPosition;
		holding = true;
		GetTree().Root.AddChild(currentHoldedObject);
	}
}
