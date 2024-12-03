using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempSealog : MonoBehaviour
{
    private Encyclopedia encyclopedia;
    private List<FishButton> cashedFish = new List<FishButton>();

    public void Scanned()
    {
        encyclopedia.ResetSealogCache(cashedFish);
        Destroy(gameObject);
    }
    public void Set(List<FishButton> fishes, Encyclopedia encyc)
    {
        cashedFish = fishes;
        encyclopedia = encyc;
    }
    public List<FishButton> GetCashedFish()
    {
        return cashedFish;
    }
}
