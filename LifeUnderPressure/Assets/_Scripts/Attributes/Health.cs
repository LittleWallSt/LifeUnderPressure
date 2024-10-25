using System;
using System.ComponentModel;
using System.Reflection;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100f;

    private Action<DamageType> onDie;
    private Action onDamage;
    private Action<float> onValueChanged;

    private float hp = 0;

    private DamageType lastDamageType;

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
                Call_OnDie(lastDamageType);
            }
            else
            {
                hp = value;
            }
            Call_OnValueChanged(hp);
            Debug.Log(lastDamageType.ToString());
        }
    }
    public float MaxHealth => maxHealth;
    public void ResetHealth()
    {
        Value = maxHealth;
    }
    private void Awake()
    {
        ResetHealth();
    }
    public void DealDamage(float damage, DamageType damageType)
    {
        if (Value <= 0f) return; 
        Value -= damage;
        lastDamageType = damageType;
        Call_OnDamage(damageType);
    }
    private void Call_OnDie(DamageType damageType)
    {
        if (onDie != null) onDie(damageType);
    }
    private void Call_OnDamage(DamageType damageType)
    {
        if (onDamage != null) onDamage();
    }
    private void Call_OnValueChanged(float value)
    {
        if (onValueChanged != null) onValueChanged(value);
    }
    public void Assign_OnDie(Action<DamageType> action)
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
    public void Remove_OnDie(Action<DamageType> action)
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

    public DamageType GetLastGamageType()
    {
        return lastDamageType;
    }
}


