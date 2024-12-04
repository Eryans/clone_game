using System.Collections.Generic;
using Godot;

[GlobalClass]
public partial class StateMachine : Node
{
    [Export]
    public State CurrentState { get; private set; }
    private readonly Dictionary<string, State> _states = new();

    public override void _Ready()
    {
        Godot.Collections.Array<Node> childrens = GetChildren();
        foreach (Node child in childrens)
        {
            if (child is State stateChild)
            {
                _states[stateChild.Name.ToString().ToLower()] = stateChild;
                stateChild.TransitionState += ChangeState;
            }
        }
        CurrentState.Enter();
    }

    public override void _Process(double delta)
    {
        if (IsInstanceValid(CurrentState)) CurrentState.Process(delta);
    }

    public override void _PhysicsProcess(double delta)
    {
        if (IsInstanceValid(CurrentState)) CurrentState.PhysicsProcess(delta);
    }

    public void ChangeState(string newState)
    {
        newState = newState.ToLower();
        if (_states[newState] == null) return;
        CurrentState.Exit();
        CurrentState = _states[newState];
        CurrentState.Enter();
    }
}
