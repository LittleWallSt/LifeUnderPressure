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
        encyclopedia = FindAnyObjectByType<Encyclopedia>();

        if (TryGetComponent<Image>(out Image image))
        {
            image.sprite = fishInfo.locked ? lockedFishImage : fishImage;
        }

        gameObject.GetComponent<Button>().onClick.AddListener(() => { encyclopedia.SetCurrentFish(fishInfo); });
    }

    
}
