using System;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float regeneratePerMinute = 25f;

    private Action<Vector3, DamageType> onDie;
    private Action onDamage;
    private Action onRespawn;
    private Action<float> onValueChanged;

    private float hp = 0;

    private DamageType lastDamageType;
    private Vector3 lastDamageDirection;
    private float lastDamageTime;

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
                Call_OnDie(lastDamageDirection, lastDamageType);
            }
            else
            {
                hp = value;
            }
            Call_OnValueChanged(hp);
        }
    }
    public float MaxHealth => maxHealth;
    public void Respawn()
    {
        ResetHealth();
        Call_OnRespawn();
    }
    public void ResetHealth()
    {
        Value = maxHealth;
    }
    private void Awake()
    {
        ResetHealth();
    }
    private void Update()
    {
        RegenerationProcess();
    }

    private void RegenerationProcess()
    {
        if (Time.time - lastDamageTime < 5) return; 
        if (Value < MaxHealth && regeneratePerMinute != 0f)
        {
            Value += regeneratePerMinute * (Time.deltaTime / 60f);
        }
    }

    public void DealDamage(float damage, Vector3 direction, DamageType damageType)
    {
        if (Value <= 0f) return; 
        Value -= damage;
        lastDamageType = damageType;
        lastDamageDirection = direction;
        lastDamageTime = Time.time;
        Call_OnDamage(damageType);
    }
    private void Call_OnDie(Vector3 direction, DamageType damageType)
    {
        if (onDie != null) onDie(direction, damageType);
    }
    private void Call_OnDamage(DamageType damageType)
    {
        if (onDamage != null) onDamage();
    }
    private void Call_OnValueChanged(float value)
    {
        if (onValueChanged != null) onValueChanged(value);
    }
    private void Call_OnRespawn()
    {
        if (onRespawn != null) onRespawn();
    }
    public void Assign_OnDie(Action<Vector3, DamageType> action)
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
    public void Assign_OnRespawn(Action action)
    {
        onRespawn += action;
    }
    public void Remove_OnDie(Action<Vector3, DamageType> action)
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
    public void Remove_OnRespawn(Action action)
    {
        onRespawn -= action;
    }

    public DamageType GetLastGamageType()
    {
        return lastDamageType;
    }
}


