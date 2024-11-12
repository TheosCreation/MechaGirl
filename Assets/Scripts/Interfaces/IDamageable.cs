using System;

public interface IDamageable
{
    float Health { get; set; }

    void Damage(float damageAmount);

    bool Heal(float healAmount);

    event Action OnDeath;
}