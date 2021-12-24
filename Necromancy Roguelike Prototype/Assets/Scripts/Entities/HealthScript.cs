using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthScript : MonoBehaviour
{
    // Script for dealing with health and damage
    [SerializeField] private int health;
    [SerializeField] private int maxHealth;
    [SerializeField] private bool playerHealth;

    [SerializeField] private bool isInvulnerable;
    
    public int Health { get { return health; } }

    public bool IsPlayerHealth { get { return playerHealth; } }

    public bool IsInvulnerable { get { return isInvulnerable; } set { isInvulnerable = value; } }
    
    public event Action<DamageInfo> OnDeath;

    public void Initialize(int maxHealth)
    {
        this.maxHealth = maxHealth;
        this.health = maxHealth;
    }

    public void Damage(int damage, DamageInfo info = new DamageInfo())
    {
        if (isInvulnerable)
            return;

        if (damage < 0)
            return;
        health -= damage;
        if (health <= 0)
        {
            OnDeath?.Invoke(info);
            Destroy(gameObject);
        }
    }

    public void Heal(int heal)
    {
        if (heal < 0)
            return;
        health += heal;
        if (health > maxHealth)
            health = maxHealth;
    }
}