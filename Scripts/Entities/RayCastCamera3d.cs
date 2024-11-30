using System;
using Godot;

public partial class RayCastCamera3d : Camera3D
{
    [Export]
    public bool activeRayShoot = true;
    public static event Action<Vector3> MouseMoved;
    public override void _PhysicsProcess(double delta)
    {
        if (activeRayShoot)
        {
            MouseMoved?.Invoke(ShootRay());
        }
    }
    public Vector3 ShootRay()
    {
        Vector2 mousePos = GetViewport().GetMousePosition();
        int rayLength = 1000;

        Vector3 from = ProjectRayOrigin(mousePos);
        Vector3 to = from + ProjectRayNormal(mousePos) * rayLength;

        var space = GetWorld3D().DirectSpaceState;
        var rayQuery = new PhysicsRayQueryParameters3D
        {
            From = from,
            To = to,
            CollisionMask = 4,
        };
        var result = space.IntersectRay(rayQuery);

        if (result.ContainsKey("position"))
        {
            return (Vector3)result["position"];
        }

        return Vector3.Zero;
    }
}