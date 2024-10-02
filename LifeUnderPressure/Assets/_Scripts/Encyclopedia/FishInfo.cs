using System;
using UnityEngine;

[CreateAssetMenu(fileName = "FishInfo", menuName = "ScriptableObjects/Fish info", order = 1)]
public class FishInfo : ScriptableObject
{
    [Header("Fish atributtes")]
    public string fishName;
    [Tooltip("Description for the preview.")]
    [TextArea(5, 15)]
    public string fishSmallDescription;
    [Tooltip("Description for the fish habitat.")]
    [TextArea(5, 15)]
    public string infoWhere;
    [Tooltip("Full description for the encyclopedia.")]
    [TextArea(15, 15)]
    public string fishFullDescription;

    public GameObject fishPrefab;

    [Tooltip("If the player unlocked the fish.")]
    public bool locked = true;

    [Tooltip("Fishing scales in meters for the preview.")]
    public float scale;

    [HideInInspector]public Action OnLockedChange;

}
