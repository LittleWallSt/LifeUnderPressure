using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Encyclopedia : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] TextMeshProUGUI fishName;
    [SerializeField] TextMeshProUGUI fishDescription;
    [SerializeField] GameObject scrollingPanel;
    
    

    [Header("ASssets")]
    [SerializeField] Transform fishDisplay;
    [SerializeField] ParticleSystem bubbles;
    GameObject currentDisplayedObj;
    FishInfo currFish;

    private static Encyclopedia instance;
    public static Encyclopedia Instance
    {
        get
        {
            if (instance == null)
            {
                Debug.LogError("Encyclopedia is null");
            }

            return instance;
        }
    }

    bool PanelOn = true;

    void Awake()
    {
        instance= this;
    }

    private void Start()
    {
        ManagePanel(true);
        
    }

    


    public void OnFishButtonClick(FishInfo fishInfo)
    {
        currFish = fishInfo;

        if (currFish == null || currFish.locked) return;
        if(currentDisplayedObj!=null)
        {
            Destroy(currentDisplayedObj);
        }
        if (currFish.fishPrefab!= null)
        {
            currentDisplayedObj = Instantiate(currFish.fishPrefab, fishDisplay.position, Quaternion.identity);
            currentDisplayedObj.transform.localScale /= currFish.scale;
            currentDisplayedObj.AddComponent<ObjectRotation>();
            addBubbles();
        }
    }



                         

    public void OnShowFullDescriptionClick()
    {
        if (PanelOn)
        {
            ManagePanel(false);
            PanelOn = false;
            if (currFish == null) return; 
            fishName.text = currFish.fishName;
            fishDescription.text = currFish.fishFullDescription;
        } else
        {
            ManagePanel(true);
            PanelOn = true;
        }
    }

    public void OnExitClick()
    {
        //do whatever
    }

    private void ManagePanel(bool On)
    {
        scrollingPanel.SetActive(On);
        fishName.gameObject.SetActive(!On);
        fishName.gameObject.SetActive(!On);
    }


    public void SetCurrentFish(FishInfo fish)
    {
        currFish = fish;
    }


    private void addBubbles()
    {
        bubbles.Play();
    }
}
