using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.TextCore.Text;
using System;

public class BeaconZone : MonoBehaviour
{
    [SerializeField] public Transform pingArea;
    [SerializeField] public float minDist;
    [SerializeField] public float maxDistForSize;
    [SerializeField] public Image UIPing;
    [SerializeField] public TMP_Text areaText; 
    [SerializeField] public TMP_Text metersText; 
    [SerializeField] public Canvas canvas;
    [SerializeField] public Camera camera;  // Player's camera

    private float minPingSize = 0.1f;
    private float maxPingSize = 0.5f;

    private float minTextSize = 0.8f;
    private float maxTextSize = 1.3f;

    private RectTransform pingTransformRect;  // Transform rect from UI ping

    void Start()
    {
        // We need the icon rect transform so we can move it
        pingTransformRect = UIPing.GetComponent<RectTransform>();

        // We hide it
        EnablePing(false);
    }

    private void FixedUpdate()
    {
        if (pingArea == null) return; //made change

        float dist = Vector3.Distance(transform.position, pingArea.position);
        dist -= minDist;

        if (metersText != null) metersText.text = string.Format("{0:0}m", (int)dist); 
    }
    void Update()
    {
        if (pingArea == null) return; //made change

        // Distance betweeen the player and the desire area
        float dist = Vector3.Distance(transform.position, pingArea.position);

        if (dist> minDist)
        {
            // If the area is inside the range, we show the icon 
            EnablePing(true);

            Vector3 areaScreenPos = camera.WorldToScreenPoint(pingArea.position); 

            // Area inside the screen vision?
            if (areaScreenPos.z > 0 && areaScreenPos.x > 0 && areaScreenPos.x < Screen.width &&
                areaScreenPos.y > 0 && areaScreenPos.y < Screen.height)
            {

                Vector3 worldSpacePos;
                // Canvas render typer world space
                RectTransformUtility.ScreenPointToWorldPointInRectangle(
                    canvas.GetComponent<RectTransform>(),
                    areaScreenPos,
                    camera,
                    out worldSpacePos);
                pingTransformRect.position = worldSpacePos;
                Debug.Log("Area IN camera vision: ");
                AdjustPingSize(dist);
            }
            else
            {
                if(!IsBehind())
                {
                    // Area outside the camera vision, then the ping is on the camera borders
                    DrawPingInBorders(areaScreenPos);
                    AdjustPingSize(dist);
                }
                else
                {
                    EnablePing(false);
                }

            }
        }
        // Area outside range
        else
        {
            EnablePing(false);
        }
    }
    private bool IsBehind()
    {
        Vector3 dir = (pingArea.position - camera.transform.position).normalized;
        Vector3 camForward = camera.transform.forward;
        camForward.y = 0;

        return Math.Abs(Vector3.SignedAngle(camForward, dir, Vector3.up)) > 90; 
    }

    private void DrawPingInBorders(Vector3 areaPos)
    {
        Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 0);
        Vector3 dirToScreen = (areaPos - screenCenter).normalized;

        // Calculation of the position in the border
        float halfScreenWidth = Screen.width / 2 - 50;  // Border adjustments
        float halfScreenHeight = Screen.height / 2 - 50;  
        float factorX = halfScreenWidth / Mathf.Abs(dirToScreen.x);
        float factorY = halfScreenHeight / Mathf.Abs(dirToScreen.y);
        float factor = Mathf.Min(factorX, factorY);
        
        Vector3 newPos = screenCenter + dirToScreen * factor; 

        // We need the pos in world space
        Vector3 worldSpacePos;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(
            canvas.GetComponent<RectTransform>(),
            newPos,
            camera,
            out worldSpacePos);

        pingTransformRect.position = worldSpacePos;
    }

    // Ping size depends on the distance between the player and the area
    
    private float AdjustSize(float dist, float maxSize, float minSize)
    {
        float scaleFactor = Mathf.InverseLerp(maxDistForSize, minDist, dist);
        float adjustedScale = Mathf.Lerp(maxSize, minSize, scaleFactor);

        return adjustedScale;
    }
    private void AdjustPingSize(float dist)
    {
        float newScale = AdjustSize(dist, maxPingSize, minPingSize);
        pingTransformRect.sizeDelta = new Vector2(newScale, newScale);

        newScale = AdjustSize(dist, maxTextSize, minTextSize);
        areaText.transform.localScale = new Vector3(newScale, newScale, 1);
        metersText.transform.localScale = new Vector3(newScale, newScale, 1);
    }
    
    public void EnablePing(bool enabled)
    {
        UIPing.enabled = enabled;
        areaText.enabled = enabled;
        metersText.enabled = enabled;
    }

    private bool IsEnable()
    {
        return UIPing.enabled;
    }



    //Added setter

    public void setPingTransform(Transform[] loc, string name = " ")
    {
        if (!IsEnable()) EnablePing(true);
        if (loc.Length == 1) setPingTransform(loc[0], name);
        else
        {
            Transform closestLoc = loc[0];
            float minDist = Vector3.Distance(transform.position, loc[0].position);

            // Itera sobre todas las ubicaciones para encontrar la más cercana
            for (int i = 1; i < loc.Length; i++)
            {
                float dist = Vector3.Distance(transform.position, loc[i].position);
                if (dist < minDist)
                {
                    minDist= dist;
                    closestLoc = loc[i];
                }
            }
            if (loc != null)
            {
                pingArea = closestLoc;
                areaText.text = name;
            }
            else Debug.Log("No reference to the area");
        }
    }

    public void setPingTransform(Transform loc, string name = " ")
    {
        if (loc != null)
        {
            pingArea = loc;
            areaText.text = name;
        }
        else Debug.Log("No reference to the area");
    }

    public void setPingTransform(Vector3 loc, string name = " ")
    {
        if (loc != null)
        {
            pingArea.position = loc;
            areaText.text = name;
        }
        else Debug.Log("No reference to the area");
    }

    public void TurnOnPing(bool enabled)
    {
        if(enabled) UIPing.gameObject.SetActive(true);
        else UIPing.gameObject.SetActive(false);
    }

    public void setMinDist(float _minDist)
    {
        minDist = _minDist;
    }

    public void setAreaName(string AreaText)
    {
        areaText.text = AreaText;
    }

}

