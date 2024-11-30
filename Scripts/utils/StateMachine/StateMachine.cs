using Godot;
using Godot.Collections;

[GlobalClass]
public partial class StateMachine : Node
{
    [Export]
    private State _currentState;
    private readonly Dictionary<string, State> _states = new();

    public override void _Ready()
    {
        Array<Node> childrens = GetChildren();
        foreach (Node child in childrens)
        {
            if (child is State stateChild)
            {
                _states[stateChild.Name.ToString().ToLower()] = stateChild;
                stateChild.TransitionState += ChangeState;
            }
        }
        _currentState.Enter();
    }

    public override void _Process(double delta)
    {
        if (IsInstanceValid(_currentState)) _currentState.Process(delta);
    }

    public override void _PhysicsProcess(double delta)
    {
        if (IsInstanceValid(_currentState)) _currentState.PhysicsProcess(delta);
    }

    public void ChangeState(string newState)
    {
        if (_states[newState] == null) return;
        _currentState.Exit();
        _currentState = _states[newState];
        _currentState.Enter();
    }
}
