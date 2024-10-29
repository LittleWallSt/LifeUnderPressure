using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BoostUI : MonoBehaviour
{
    [SerializeField] Slider boostSlider;
    [SerializeField] TextMeshProUGUI CDText;

    public void UpdateUI(float value, float CD)
    {
        boostSlider.value = value;
        CDText.text = "Boost CD: " + CD;
    }
}
