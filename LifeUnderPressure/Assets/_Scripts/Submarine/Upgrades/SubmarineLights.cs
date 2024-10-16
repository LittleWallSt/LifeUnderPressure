using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubmarineLights : SubmarineUpgrade
{
    [SerializeField] private KeyCode turnLightsKey = KeyCode.F;
    [Header("Lights")]
    [SerializeField] private float[] lightIntensity = null;
    [SerializeField] private Light[] submarineLights = null;

    [ContextMenu("SU_EditorSetup")]
    protected override void SU_EditorSetup()
    {
        base.SU_EditorSetup();

        if (lightIntensity.Length != maxLevel + 1)
        {
            lightIntensity = new float[maxLevel + 1];
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(turnLightsKey))
        {
            foreach(Light light in submarineLights)
            {
                light.gameObject.SetActive(!light.gameObject.activeSelf);
            }
        }
    }
    public override void UpgradeLevel()
    {
        base.UpgradeLevel();

        foreach(Light light in submarineLights)
        {
            light.intensity = lightIntensity[level];
        }
    }
}
