using System;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100f;

    private Action onDie;

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

    private void Start()
    {
        hp = maxHealth;
    }
    public void DealDamage(float damage)
    {
        Value -= damage;
    }
    private void Call_OnDie()
    {
        if (onDie != null) onDie();
    }
    public void Assign_OnDie(Action action)
    {
        onDie += action;
    }
}
