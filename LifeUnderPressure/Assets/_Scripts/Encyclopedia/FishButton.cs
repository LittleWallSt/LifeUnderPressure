using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FishButton : MonoBehaviour
{
    [SerializeField] Sprite fishImage;
    [SerializeField] Sprite lockedFishImage;
    public FishInfo fishInfo;
    private Encyclopedia encyclopedia;
    private void Start()
    {
        fishInfo.OnLockedChange += ChangeImage;
        encyclopedia = FindAnyObjectByType<Encyclopedia>();

        if (TryGetComponent<Image>(out Image image))
        {
            image.sprite = fishInfo.locked ? lockedFishImage : fishImage;
        }

        gameObject.GetComponent<Button>().onClick.AddListener(() => { encyclopedia.SetCurrentFish(fishInfo); });
        
        // Aleksis >>
        GameManager.Instance.Assign_OnDataLoaded(LoadFishInfo);
        // Aleksis <<
    }
    void ChangeImage()
    {
        if (TryGetComponent<Image>(out Image image))
        {
            image.sprite = fishImage; 
        }
    }

    // Aleksis >>
    private void LoadFishInfo()
    {
        fishInfo.locked = DataManager.Get("FishScanned_" + fishInfo.name, 0) == 1 ? false : true;
        GameManager.Instance.Remove_OnDataLoaded(LoadFishInfo);
    }
    // Aleksis <<
}
