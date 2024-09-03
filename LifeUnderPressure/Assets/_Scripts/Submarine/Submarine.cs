using TMPro;
using UnityEngine;

[RequireComponent(typeof(SubmarineMovement))]
public class Submarine : MonoBehaviour
{
    [SerializeField] private TMP_Text heightText = null;

    private SubmarineMovement movement;

    private void Awake()
    {
        movement = GetComponent<SubmarineMovement>();
    }
    private void FixedUpdate()
    {
        float depth = -transform.position.y;
        if (heightText) heightText.text = string.Format("Depth: {0:F1}", depth);
    }
}
