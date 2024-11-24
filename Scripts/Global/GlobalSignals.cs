using Godot;
using System;

public partial class GlobalSignals : Node
{
	public static GlobalSignals Instance { get; private set; }
	[Signal]
	public delegate void World3DMousePositionEventHandler(Vector3 mousePos);

	public override void _Ready()
	{
		Instance = this;
	}
	public void EmitNewWorld3DMousePosition(Vector3 mousePos)
	{
		EmitSignal(nameof(World3DMousePosition), mousePos);
	}
}
