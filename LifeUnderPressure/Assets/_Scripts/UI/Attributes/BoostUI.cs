using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BoostUI : MonoBehaviour
{
    [SerializeField] Slider boostSlider;
    [SerializeField] TextMeshProUGUI CDText;

    private void Start()
    {
        boostSlider.value = 0;
    }
    public void UpdateUI(float value, float CD)
    {
        boostSlider.value = value;
        CDText.text = "Boost CD: " + Mathf.Round(CD * 10.0f) * 0.1f;
    }
}
