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
    [SerializeField] TextMeshProUGUI whereToFind;
    [SerializeField] GameObject scrollingPanel;
    [SerializeField] GameObject scrollingText;
    
    

    [Header("Assets")]
    [SerializeField] Transform fishDisplayOffset;
    [SerializeField] Transform bubblesOffset;
    [SerializeField] ParticleSystem bubbles;


    GameObject currentDisplayedObj;
    GameObject submarineBody;
    FishInfo currFish;

    

    bool PanelOn = true;

    

    private void Start()
    {
        ManagePanel(true);
        gameObject.SetActive(false);
        whereToFind.text = "";
    }

    


    public void OnFishButtonClick(FishInfo fishInfo)
    {
        whereToFind.text = "";
        Debug.Log("cli");
        currFish = fishInfo;

        if (currFish == null) return;
        if(currentDisplayedObj!=null)
        {
            Destroy(currentDisplayedObj);
            currentDisplayedObj = null;
        }
        if (currFish.locked)
        {
            ShowWhereInfo();
            return;
        } 
        if (currFish.fishPrefab!= null)
        {
            currentDisplayedObj = Instantiate(currFish.fishPrefab, Vector3.zero, Quaternion.identity, fishDisplayOffset);
            currentDisplayedObj.transform.localPosition = Vector3.zero;
            currentDisplayedObj.transform.localScale /= currFish.scale;
            currentDisplayedObj.AddComponent<ObjectRotation>();
            addBubbles();
        }

        
    }

    


    public void ShowWhereInfo()
    {
        whereToFind.text = currFish.infoWhere;
    }


    public void OnShowFullDescriptionClick()
    {
        fishName.text = ""; 
        fishDescription.text = "";
        if (PanelOn)
        {
            ManagePanel(false);
            PanelOn = false;
            whereToFind.text = "";
            if (currFish == null || currFish.locked) return; 
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
        EnableMenu(false, submarineBody); 
    }

    // Aleksis >> changed from void to bool, added return state;
    public bool EnableMenu(bool state, GameObject _submarineBody)
    {
        Destroy(currentDisplayedObj);
        if (submarineBody == null) submarineBody = _submarineBody;
        currentDisplayedObj = null;
        gameObject.SetActive(state);
        submarineBody.SetActive(!state);
        InternalSettings.EnableCursor(gameObject.activeSelf);
        return state;
    }

    private void ManagePanel(bool On)
    {
        scrollingPanel.SetActive(On);
        scrollingText.SetActive(!On);
    }


    public void SetCurrentFish(FishInfo fish)
    {
        currFish = fish;
    }


    private void addBubbles()
    {
        
        bubbles.gameObject.transform.position = bubblesOffset.position;
        bubbles.Play();
    }
}
