using Godot;
using System;

[GlobalClass]
public partial class State : Node
{
    public event Action<string> TransitionState;
    public virtual void Enter() { }
    public virtual void Exit() { }
    public virtual void Process(double delta) { }
    public virtual void PhysicsProcess(double delta) { }
    public void ChangeToState(string newState)
    {
        TransitionState?.Invoke(newState.ToLower());
    }
}
