using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SonarUI : MonoBehaviour
{
    [SerializeField] Canvas canvas;
    [SerializeField] Image pointPrefab;
    [SerializeField] RectTransform arrow;
    [SerializeField] SonarRadar sonar;


    [Header("Colours")]
    [SerializeField] Color normal;
    [SerializeField] Color fishUp;
    [SerializeField] Color fishDown;
    [SerializeField] float fadeDuration = 2f;

    public GameObject randFish;

    

    float screenWidth;
    float screenHeight;


    void Start()
    {
        sonar.sonarBeep += DrawFish;
        screenWidth = canvas.GetComponent<RectTransform>().sizeDelta.x;

    }

    private void DrawFish(float distPercentage, Vector3 direction)
    {
        Image fishPoint = DrawCircle(distPercentage, direction);
        StartCoroutine(Fade(1f, 0f, fishPoint, true));
        
    }

    private Image DrawCircle(float distPercentage, Vector3 direction)
    {
        float rad = screenWidth/2*distPercentage;
        Debug.Log(distPercentage);
        Vector3 circlePos =  new Vector3(rad*direction.x, rad*direction.z, 0);
        Image point = Instantiate(pointPrefab, Vector3.zero, Quaternion.identity);
        Vector3 originalSize = point.rectTransform.localScale;
        Quaternion originalRot = point.rectTransform.localRotation;

        point.transform.SetParent(transform);
        point.rectTransform.localScale = originalSize;
        point.rectTransform.localRotation= originalRot;
        point.rectTransform.localPosition = circlePos;
        point.color = CircleColor(direction);
        Debug.Log(circlePos);
        
        return point;

    }

    private IEnumerator Fade(float startAlpha, float endAlpha, Image imageToFade, bool destroy = false)
    {
        Color imageColor = imageToFade.color;

        for (float t = 0f; t < fadeDuration; t += Time.deltaTime)
        {
            float normalizedTime = t / fadeDuration;
            imageColor.a = Mathf.Lerp(startAlpha, endAlpha, normalizedTime); 
            imageToFade.color = imageColor; 
            yield return null; 
        }
        imageColor.a = endAlpha;
        imageToFade.color = imageColor;

        if (destroy)
        {
            Destroy(imageToFade.gameObject);
        }

    }



    private Color CircleColor(Vector3 direction)
    {
        return direction.y > 0 ? fishUp : fishDown;
    }

    
}
