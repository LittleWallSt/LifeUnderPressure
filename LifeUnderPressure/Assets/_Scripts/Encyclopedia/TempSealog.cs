using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempSealog : MonoBehaviour
{
    private Encyclopedia encyclopedia;
    private List<FishInfo> cashedFish = new List<FishInfo>();

    public void Scanned()
    {
        encyclopedia.ResetSealogCache(cashedFish);
        Destroy(gameObject);
    }
    public void Set(List<FishInfo> fishes, Encyclopedia encyc)
    {
        cashedFish = fishes;
        encyclopedia = encyc;
    }
    public List<FishInfo> GetCashedFish()
    {
        return cashedFish;
    }
}
