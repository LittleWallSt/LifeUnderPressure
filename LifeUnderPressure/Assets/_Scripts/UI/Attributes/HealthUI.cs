using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    [SerializeField] private Health health = null;
    [SerializeField] private Slider slider = null;
    [SerializeField] private float highlightTimer = 0.5f;
    [SerializeField] private float highlightFrequency = 0.05f;
    [SerializeField] private Image sliderBackground = null;
    [SerializeField] private Color highlightColor = Color.white;
    [SerializeField] private Color defaultColor = Color.white;

    private float highlightTime = 0f;
    private bool highlighting = false;
    private void Start()
    {
        health.Assign_OnDamage(UpdateUI);
        slider.maxValue = health.MaxHealth;
        slider.value = health.Value;
    }
    private void UpdateUI()
    {
        if (slider.value > health.Value)
        {
            if (highlighting)
            {
                highlightTime = 0f;
            }
            else
            {
                StartCoroutine(HighlightHealthLoss());
            }
        }
        slider.value = health.Value;
    }
    private IEnumerator HighlightHealthLoss()
    {
        highlightTime = 0f;
        highlighting = true;
        while (highlightTime < highlightTimer)
        {
            sliderBackground.color = sliderBackground.color == defaultColor ? highlightColor : defaultColor;
            yield return new WaitForSeconds(highlightFrequency);
            highlightTime += highlightFrequency;
        }
        highlighting = false;
    }
}
