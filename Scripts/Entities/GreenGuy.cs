using Godot;

public partial class GreenGuy : CharacterBody3D
{
    private PlayerAnimationTree _animationTree;
    public override void _Ready()
    {
        _animationTree = GetNode<PlayerAnimationTree>("AnimationTree");
    }

}
