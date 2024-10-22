using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Encyclopedia : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] TextMeshProUGUI fishName;
    [SerializeField] TextMeshProUGUI smallDescription;

    [SerializeField] TextMeshProUGUI fishDescription;
    [SerializeField] GameObject lockImage;

    

    [Header("Images")]
    [Tooltip("Fish's states (None, Marked, Scanned)")]
    public Sprite[] FishStates;
    [Header("Colour")]
    public Color inQuestColor;

    [Header("Ping")]
    public BeaconZone ping;
    

    FishButton[] fishes;
    List<FishButton> cashedFish = new List<FishButton>();
    GameObject submarineBody;

    // Aleksis >>
    private static Action OnCurrentFishChanged;

    public static FishButton CurrFish 
    {
        get
        {
            return currFish;
        } 
        private set
        {
            if (currFish != value)
            {
                currFish = value;
                Call_OnCurrentFishChanged();
            }
        }
    }
    // Aleksis <<
    private static FishButton currFish;


    private void Start()
    {
        ClearText();
        lockImage.SetActive(true);
        gameObject.SetActive(false);
        currFish = null;
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
            EnableMenu(false, submarineBody);
    }

    void ClearText()
    {
        fishName.text = "";
        smallDescription.text = "";
        fishDescription.text = "";
    }


    public void OnFishButtonClick(FishButton fishButton)
    {
        FishInfo fishInfo = fishButton.fishInfo;
        fishName.text = fishInfo.name;
        smallDescription.text = fishInfo.infoWhere;
        if (currFish!=null)
        {
            if (currFish.GetFishState() != FishState.Scanned)
            {
                currFish.SetFishState(FishState.None);
                currFish.SetIcon();
            }
                
        }
        CurrFish = fishButton;
        ShowFullDescription(fishButton.GetFishState() == FishState.Scanned, fishInfo);
        ShowTheBeacon(fishInfo);
    }



    private void ShowTheBeacon(FishInfo fish)
    {
        //need beacon
    }

    private void ShowFullDescription(bool on, FishInfo fishInfo)
    {
        lockImage.SetActive(!on);
        fishDescription.gameObject.SetActive(on);
        if (on)
        {
            fishDescription.text = fishInfo.fishFullDescription;
        }
    }

    private void MenuUpdate()
    {
        if (currFish != null)
        {
            ShowFullDescription(currFish.GetFishState() == FishState.Scanned, currFish.fishInfo);
        }

    } 




    // Aleksis >> changed from void to bool, added return state;
    public bool EnableMenu(bool state, GameObject _submarineBody)
    {
        if (submarineBody == null) submarineBody = _submarineBody;
        gameObject.SetActive(state);
        submarineBody.SetActive(!state);
        InternalSettings.EnableCursor(gameObject.activeSelf);
        if (state)
        {
            UpdateIcons();
            MenuUpdate(); 
        }
        return state;
    }

    void UpdateIcons()
    {
        if (fishes==null || fishes.Length <=0) fishes = FindObjectsOfType<FishButton>();
        if (fishes.Length > 0)
        {
            foreach(var fish in fishes)
            {
                fish.SetIcon();
                fish.fishInfo.locked = true;
            }
        }
    }

    public void ClearSealog()
    {
        cashedFish.Clear();
        if (fishes == null || fishes.Length <= 0) fishes = FindObjectsOfType<FishButton>();
        if (fishes.Length > 0 )
        {
            foreach (FishButton fish in fishes)
            {
                if (fish.GetFishState()==FishState.Scanned)
                {
                    cashedFish.Add(fish);
                    fish.SetFishState(FishState.None);
                    fish.SetIcon();
                }
            }
        }
    }

    public void ResetSealogCache()
    {
        if (cashedFish!=null && cashedFish.Count>0)
        {
            foreach(FishButton fish in cashedFish)
            {
                fish.SetFishState(FishState.Scanned);
                fish.SetIcon();
            }
        }
    }

    // Actions
    // Aleksis >>
    public static void Assign_OnCurrentFishChanged(Action action)
    {
        OnCurrentFishChanged += action;
    }
    public static void Remove_OnCurrentFishChanged(Action action)
    {
        OnCurrentFishChanged -= action;
    }
    private static void Call_OnCurrentFishChanged()
    {
        if(OnCurrentFishChanged != null) OnCurrentFishChanged();
    }
    // Aleksis <<




}
