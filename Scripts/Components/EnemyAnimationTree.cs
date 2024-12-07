using Godot;
using System;

public partial class EnemyAnimationTree : AnimationTree
{
    [Export]
    public float BlendSpeed = 15f;
    public enum AnimationState { IDLE, WALK, DEATH, HIT }
    private AnimationState _currentAnimation = AnimationState.IDLE;
    private CharacterBody3D parent;

    private float _idleBlendValue = 1f;
    private float _walkBlendValue = 0f;
    private float _holdBlendValue = 0f;
    private float _deathBlendValue = 0f;
    private float _hitBlendValue = 0f;

    public override void _Ready()
    {
        parent = GetParent<CharacterBody3D>();
    }
    public override void _Process(double delta)
    {
        HandleAnimation(delta);
        UpdateTree();
    }
    public void SetCurrentAnimation(AnimationState animationName)
    {
        _currentAnimation = animationName;
    }
    public void HandleAnimation(double delta)
    {
        switch (_currentAnimation)
        {
            case AnimationState.IDLE:
                _idleBlendValue = Mathf.Lerp(_idleBlendValue, 1f, (float)(BlendSpeed * delta));
                _walkBlendValue = Mathf.Lerp(_walkBlendValue, 0f, (float)(BlendSpeed * delta));
                _hitBlendValue = Mathf.Lerp(_hitBlendValue, 0f, (float)(BlendSpeed * delta));
                break;

            case AnimationState.WALK:
                _idleBlendValue = Mathf.Lerp(_idleBlendValue, 0f, (float)(BlendSpeed * delta));
                _walkBlendValue = Mathf.Lerp(_walkBlendValue, 1f, (float)(BlendSpeed * delta));
                _hitBlendValue = Mathf.Lerp(_hitBlendValue, 0f, (float)(BlendSpeed * delta));
                break;
            case AnimationState.HIT:
                _idleBlendValue = Mathf.Lerp(_idleBlendValue, 0f, (float)(BlendSpeed * delta));
                _walkBlendValue = Mathf.Lerp(_walkBlendValue, 0f, (float)(BlendSpeed * delta));
                _hitBlendValue = Mathf.Lerp(_hitBlendValue, 1f, (float)(BlendSpeed * delta));
                break;
            case AnimationState.DEATH:
                _idleBlendValue = Mathf.Lerp(_idleBlendValue, 0f, (float)(BlendSpeed * delta));
                _walkBlendValue = Mathf.Lerp(_walkBlendValue, 0f, (float)(BlendSpeed * delta));
                _hitBlendValue = Mathf.Lerp(_hitBlendValue, 0f, (float)(BlendSpeed * delta));
                _deathBlendValue = Mathf.Lerp(_deathBlendValue, 1f, (float)(BlendSpeed * delta));
                break;
        }

    }

    public void UpdateTree()
    {
        Set("parameters/WalkBlend/blend_amount", _walkBlendValue);
        Set("parameters/DeathBlend/blend_amount", _deathBlendValue);
        Set("parameters/HitBlend/blend_amount", _hitBlendValue);
    }
}
