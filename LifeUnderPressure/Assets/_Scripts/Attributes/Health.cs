using System;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100f;

    private Action onDie;
    private Action onDamage;
    private Action<float> onValueChanged;

    private float hp = 0;

    public float Value
    {
        get
        {
            return hp;
        }
        private set
        {
            if (value == hp) return;

            if (value <= 0f)
            {
                hp = 0f;
                Call_OnDie();
            }
            else
            {
                hp = value;
            }
            Call_OnValueChanged(hp);
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
    private void Call_OnDamage()
    {
        if (onDamage != null) onDamage();
    }
    private void Call_OnValueChanged(float value)
    {
        if (onValueChanged != null) onValueChanged(value);
    }
    public void Assign_OnDie(Action action)
    {
        onDie += action;
    }
    public void Assign_OnDamage(Action action)
    {
        onDamage += action;
    }
    public void Assign_OnValueChanged(Action<float> action)
    {
        onValueChanged += action;
    }
    public void Remove_OnDie(Action action)
    {
        onDie -= action;
    }
    public void Remove_OnDamage(Action action)
    {
        onDamage -= action;
    }
    public void Remove_OnValueChanged(Action<float> action)
    {
        onValueChanged -= action;
    }
}
