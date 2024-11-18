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

    [SerializeField] GameObject firstHighlight;

    [Header("Images")]
    [Tooltip("Fish's states (None, Marked, Scanned)")]
    public Sprite[] FishStates;
    [Header("Colour")]
    public Color inQuestColor;

    [Header("Ping")]
    public BeaconZone ping;

    bool firstTime = true;
    

    FishButton[] fishes;
    List<FishButton> cashedFish = new List<FishButton>();
    GameObject submarineBody;
    FishInfo fishInfo; 
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

        ping.onInArea += PlayShortVoiceline;
        lockImage.SetActive(true);
        gameObject.SetActive(false);
        currFish = null;

        
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
            EnableMenu(false, submarineBody);
    }

    void PlayShortVoiceline()
    {
        if (currFish == null || currFish.fishInfo == null) return;
        if (currFish.fishInfo.shortVO!= null && !currFish.shortPlayed)
        {
            VoicelinesUI.Instance.CallVoiceline(currFish.fishInfo.shortVO);
            currFish.shortPlayed = true; 
        }

    }

    void ClearText()
    {
        fishName.text = "";
        smallDescription.text = "";
        fishDescription.text = "";
    }


    public void OnFishButtonClick(FishButton fishButton)
    {
        // >> Javi
        fishButton.no = false;

        if (!firstTime&& fishInfo == fishButton.fishInfo)
        {
            fishButton.no = true;
            currFish.SetIcon();
            if (fishButton.GetFishState() != FishState.Scanned)
            {
                fishButton.SetFishState(FishState.None);
            }
            ping.pingArea = null;
            fishName.text = "";
            smallDescription.text = "";
            ShowFullDescription(fishButton.GetFishState() == FishState.Scanned, fishInfo);
            fishDescription.text = "";
            ShowTheBeacon(fishInfo);
            ping.EnablePing(false);
            UpdateIcons();
            

            fishInfo = null; 
            return; 
        }
        // << Javi

        if (firstTime)
        {
            firstTime = false;
            if (firstHighlight!=null) firstHighlight.SetActive(false);
        }
        fishInfo = fishButton.fishInfo;
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
        //submarineBody.SetActive(!state);  ??
        Submarine.Instance.getSubmarineMovement().StopSubmarine();
        Submarine.Instance.getSubmarineMovement().enabled = !state;
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
