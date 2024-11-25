using Godot;
using System;

public partial class PlayerAnimationTree : AnimationTree
{
    [Export]
    public float BlendSpeed = 15f;
    public bool isHolding = false;
    private enum AnimationState { IDLE, WALK, HOLD }
    private AnimationState _currentAnimation = AnimationState.IDLE;
    private CharacterBody3D parent;

    private float _idleBlendValue = 1f;
    private float _walkBlendValue = 0f;
    private float _holdBlendValue = 0f;

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
                _idleBlendValue = Mathf.Lerp(_idleBlendValue, 1f, (float)(BlendSpeed * delta));
                _walkBlendValue = Mathf.Lerp(_walkBlendValue, 0f, (float)(BlendSpeed * delta));
                break;

            case AnimationState.WALK:
                _idleBlendValue = Mathf.Lerp(_idleBlendValue, 0f, (float)(BlendSpeed * delta));
                _walkBlendValue = Mathf.Lerp(_walkBlendValue, 1f, (float)(BlendSpeed * delta));
                break;
        }
        if (isHolding)
        {
            _holdBlendValue = Mathf.Lerp(_holdBlendValue, 1f, (float)(BlendSpeed * delta));
        }
        else
        {
            _holdBlendValue = Mathf.Lerp(_holdBlendValue, 0f, (float)(BlendSpeed * delta));
        }
    }

    public void UpdateTree()
    {
        Set("parameters/WalkBlend/blend_amount", _walkBlendValue);
        Set("parameters/HoldBlend/blend_amount", _holdBlendValue);
    }
}
