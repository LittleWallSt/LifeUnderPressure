using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    [SerializeField] private Health health = null;
    [SerializeField] private Slider slider = null;

    private void Start()
    {
        health.Assign_OnDamage(UpdateUI);
        slider.maxValue = health.MaxHealth;
        slider.value = health.Value;
    }
    private void UpdateUI()
    {
        slider.value = health.Value;
    }
}
