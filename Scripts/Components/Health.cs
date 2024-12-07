using Godot;
using System;

[GlobalClass]
public partial class Health : Node
{
    [Export]
    private int _maxHealth = 10;
    [Export]
    private int _currentHealth;

    public event Action EntityDie;
    public event Action EntityHit;
    public override void _Ready()
    {
        _currentHealth = _maxHealth;
    }
    public bool IsAlive()
    {
        return _currentHealth > 0;
    }

    public void TakeDamage(int damage)
    {
        _currentHealth -= damage;
        if (_currentHealth <= 0) { EntityDie?.Invoke(); } else { EntityHit?.Invoke(); }
    }

}
