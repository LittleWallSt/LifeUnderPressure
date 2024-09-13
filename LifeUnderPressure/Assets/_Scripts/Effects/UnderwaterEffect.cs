using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnderwaterEffect : MonoBehaviour
{
    [SerializeField] private float maxDepth = 100f;
    [SerializeField] private Color colorTop;
    [SerializeField] private Color colorBottom;
    [SerializeField] private float densityTop = 0f;
    [SerializeField] private float densityBottom = 0f;


    private Transform player;

    private void Start()
    {
        player = Submarine.Instance.transform;
    }
    private void Update()
    {
        float depth = -player.transform.position.y;
        float value = depth / maxDepth;
        RenderSettings.fogColor = Color.Lerp(colorTop, colorBottom, value);
        RenderSettings.fogDensity = Mathf.Lerp(densityTop, densityBottom, value);
    }
}
