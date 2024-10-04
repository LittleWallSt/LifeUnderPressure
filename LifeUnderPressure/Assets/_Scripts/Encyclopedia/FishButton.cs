using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FishButton : MonoBehaviour
{

    [Header("UI")]
    [SerializeField] Sprite fishImage;
    [SerializeField] Sprite lockedFishImage;
    [SerializeField] Image stateIcon;
    [SerializeField] Image fishIcon;

    
    public FishInfo fishInfo;
    public Transform habitat;
    private Encyclopedia encyclopedia;

    private FishState fishState;
    private bool inQuest;

    private void Start()
    {
        fishInfo.OnLockedChange += ChangeImage;
        encyclopedia = FindAnyObjectByType<Encyclopedia>();

        if (TryGetComponent<Image>(out Image image))
        {
            image.sprite = fishInfo.locked ? lockedFishImage : fishImage;
        }

        //gameObject.GetComponent<Button>().onClick.AddListener(() => { encyclopedia.SetCurrentFish(fishInfo); });
        gameObject.GetComponent<Button>().onClick.AddListener(() => { encyclopedia.ping.setPingTransform(habitat);
            fishState = FishState.Marked;
            SetIcon();
        });

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

    public void SetIcon()
    {
        if (fishState != FishState.Scanned) ChangeStateToScanned(fishInfo);
        if (inQuest) fishIcon.color = encyclopedia.inQuestColor;
        else fishIcon.color = Color.white;
        switch(fishState)
        {
            case FishState.None:
                stateIcon.sprite = encyclopedia.FishStates[0];
                break;
            case FishState.Marked:
                stateIcon.sprite = encyclopedia.FishStates[1];
                break;
            case FishState.Scanned:
                stateIcon.sprite = encyclopedia.FishStates[2];
                break;
        }
    }

    private void ChangeStateToScanned(FishInfo info)
    {
        if(info.locked == false) fishState= FishState.Scanned;
    }

    // Aleksis >>
    private void LoadFishInfo()
    {
        fishInfo.locked = DataManager.Get("FishScanned_" + fishInfo.name, 0) == 1 ? false : true;
        GameManager.Instance.Remove_OnDataLoaded(LoadFishInfo);
    }
    // Aleksis <<

    public void SetInQuest(bool _inQuest)
    {
        inQuest= _inQuest;
    }

    public bool GetInQuest() 
    { 
        return inQuest; 
    }

    public void SetFishState(FishState _fishState)
    {
        fishState = _fishState;
    }

    public FishState GetFishState()
    {
        return fishState;
    }
}




public enum FishState
{
    None,
    Marked,
    Scanned,
}
