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
    }

    


    public void OnFishButtonClick(FishInfo fishInfo)
    {
        Debug.Log("cli");
        currFish = fishInfo;

        if (currFish == null || currFish.locked) return;
        if(currentDisplayedObj!=null)
        {
            Destroy(currentDisplayedObj);
            currentDisplayedObj = null;
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
        EnableMenu(false, submarineBody); 
    }

    public void EnableMenu(bool state, GameObject _submarineBody)
    {
        Destroy(currentDisplayedObj);
        if (submarineBody == null) submarineBody = _submarineBody;
        currentDisplayedObj = null;
        gameObject.SetActive(state);
        submarineBody.SetActive(!state);
        InternalSettings.EnableCursor(gameObject.activeSelf);
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
        
        bubbles.gameObject.transform.position = bubblesOffset.position;
        bubbles.Play();
    }
}
