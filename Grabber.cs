using Godot;
using System;
using System.Collections.Generic;

public partial class Grabber : RayCast3D
{
	[Export]
	public AudioStreamPlayer YeetSound;
	[Export]
	private Vector3 yeetDirection = new(0, 15, -5);
	[Export]
	private int yeetPreviewStep = 40;
	private CharacterBody3D parent;
	private Marker3D grabbedObjectPoint;

	private Throwable currentHoldedObject;
	private PackedScene _throwableScene;
	private MeshInstance3D yeetPreviewer = new();
	private Material yeetPreviewMaterial;
	public override void _Ready()
	{
		parent = GetParent<CharacterBody3D>();
		grabbedObjectPoint = GetNode<Marker3D>("Marker3D");
		_throwableScene = GD.Load<PackedScene>("res://Nodes/Components/throwable.tscn");
		AddChild(yeetPreviewer);
		yeetPreviewer.Mesh = new ImmediateMesh();
		yeetPreviewMaterial = GD.Load<Material>("res://Assets/Materials/preview_mat.tres");

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
		YeetPreview(delta);
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
			currentHoldedObject.Yeet(parent.Transform.Basis * yeetDirection);
			currentHoldedObject = null;
		}
	}

	private void YeetPreview(double delta)
	{
		ImmediateMesh mesh = (ImmediateMesh)yeetPreviewer.Mesh;
		mesh.ClearSurfaces();

		mesh.SurfaceBegin(Mesh.PrimitiveType.Lines);
		mesh.SurfaceSetColor(new Color(1, 0, 0, 1));
		yeetPreviewer.SetSurfaceOverrideMaterial(0, yeetPreviewMaterial);
		Vector3 startPosition = grabbedObjectPoint.Transform.Origin;

		Vector3 initialVelocity = yeetDirection;

		Vector3 gravity = parent.GetGravity();

		float accumulatedTime = 0.0f;

		Vector3 previousPosition = startPosition;

		for (int i = 0; i < yeetPreviewStep; i++)
		{
			accumulatedTime += (float)delta;

			Vector3 currentPosition = startPosition + initialVelocity * accumulatedTime + 0.5f * gravity * accumulatedTime * accumulatedTime;

			mesh.SurfaceAddVertex(previousPosition);
			mesh.SurfaceAddVertex(currentPosition);

			previousPosition = currentPosition;
		}

		mesh.SurfaceEnd();
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
