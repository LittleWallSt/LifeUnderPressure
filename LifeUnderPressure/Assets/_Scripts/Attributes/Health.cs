using System;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100f;

    private Action onDie;
    private Action onDamage;

    private float hp = 0;

    public float Value
    {
        get
        {
            return hp;
        }
        private set
        {
            if (value <= 0f)
            {
                hp = 0f;
                Call_OnDie();
            }
            else
            {
                hp = value;
            }
        }
    }
    public float MaxHealth => maxHealth;
    public void ResetHealth()
    {
        hp = maxHealth;
    }
    private void Awake()
    {
        ResetHealth();
    }
    public void DealDamage(float damage)
    {
        Value -= damage;
        Call_OnDamage();
    }
    private void Call_OnDie()
    {
        if (onDie != null) onDie();
    }
    public void Assign_OnDie(Action action)
    {
        onDie += action;
    }
    private void Call_OnDamage()
    {
        if (onDamage != null) onDamage();
    }
    public void Assign_OnDamage(Action action)
    {
        onDamage += action;
    }
}
