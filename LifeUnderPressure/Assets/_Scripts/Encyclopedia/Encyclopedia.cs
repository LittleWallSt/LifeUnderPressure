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

    GameObject submarineBody;
    FishInfo currFish;


    private void Start()
    {
        ClearText();
        lockImage.SetActive(true);
        gameObject.SetActive(false);
        
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

        currFish = fishInfo;
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




    // Aleksis >> changed from void to bool, added return state;
    public bool EnableMenu(bool state, GameObject _submarineBody)
    {
        if (submarineBody == null) submarineBody = _submarineBody;
        gameObject.SetActive(state);
        submarineBody.SetActive(!state);
        InternalSettings.EnableCursor(gameObject.activeSelf);
        if (state) UpdateIcons();
        return state;
    }

    void UpdateIcons()
    {
        if (fishes==null || fishes.Length <=0) fishes = FindObjectsOfType<FishButton>();
        if (fishes.Length > 0)
        {
            foreach(var fish in fishes)
            {
                fish.SetIcon(fish.fishInfo);
            }
        }
    }



    public void SetCurrentFish(FishInfo fish)
    {
        currFish = fish;
    }



}
