using Godot;
using System;

public partial class PlayerAnimationTree : AnimationTree
{
    [Export]
    public float BlendSpeed = 15f;

    private enum AnimationState { IDLE, WALK }
    private AnimationState _currentAnimation = AnimationState.IDLE;
    private CharacterBody3D parent;

    private float _IdleBlendValue = 1f;
    private float _WalkBlendValue = 0f;

    public override void _Ready()
    {
        parent = GetParent<CharacterBody3D>();
    }
    public override void _Process(double delta)
    {
        HandleAnimation(delta);
        UpdateTree();
        if (parent.Velocity.Length() > 0)
        {
            _currentAnimation = AnimationState.WALK;
        }
        else
        {
            _currentAnimation = AnimationState.IDLE;
        }
    }

    public void HandleAnimation(double delta)
    {
        switch (_currentAnimation)
        {
            case AnimationState.IDLE:
                _IdleBlendValue = Mathf.Lerp(_IdleBlendValue, 1f, (float)(BlendSpeed * delta));
                _WalkBlendValue = Mathf.Lerp(_WalkBlendValue, 0f, (float)(BlendSpeed * delta));
                break;

            case AnimationState.WALK:
                _IdleBlendValue = Mathf.Lerp(_IdleBlendValue, 0f, (float)(BlendSpeed * delta));
                _WalkBlendValue = Mathf.Lerp(_WalkBlendValue, 1f, (float)(BlendSpeed * delta));
                break;
        }
    }

    public void UpdateTree()
    {
        Set("parameters/WalkBlend/blend_amount", _WalkBlendValue);
    }
}
