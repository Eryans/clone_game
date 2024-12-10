using Godot;

public partial class GreenGuy : CharacterBody3D
{
    private PlayerAnimationTree _animationTree;
    private Grabber _grabber;
    public override void _Ready()
    {
        _animationTree = GetNode<PlayerAnimationTree>("AnimationTree");
        _grabber = GetNode<Grabber>("Grabber");
    }

    public override void _Process(double delta)
    {
        _animationTree.isHolding = _grabber.Armed;
    }
}
