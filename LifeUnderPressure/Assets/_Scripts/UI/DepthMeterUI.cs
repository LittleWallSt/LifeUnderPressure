using System.Collections;
using TMPro;
using UnityEngine;

public class DepthMeterUI : MonoBehaviour
{
    [SerializeField]TextMeshProUGUI[] depthMeters;
    [SerializeField]RectTransform depthTransform;

    [SerializeField] float cellHeight = 30f;
    float rollDuration = 0.08f;
    [SerializeField] int _sunsetOffset = 20;
    [SerializeField] int _twiglightOffset = 50;  
    [SerializeField] int _midnightOffset = 150;

    [SerializeField] float twiglightZoneDepth = 200f;
    [SerializeField] float midnightZoneDepth = 900f;

    int previousThreshold;
    float previousDepth;


    private float smoothSpeed = 100f;

    Vector2 basePos;
    Vector2 targetPos;

    private void Start()
    {
        previousThreshold = ((int)Submarine.Instance.GetSubmarineDepth()/_sunsetOffset)*_sunsetOffset;
        basePos = depthTransform.anchoredPosition;
        ChangeThreshold(_sunsetOffset);
    }

    private void FixedUpdate()
    {
        //DepthCheck();
        SmoothDepthCheck();
    }

    private void SmoothDepthCheck()
    {
        int offset = (int)Submarine.Instance.GetSubmarineDepth() >= twiglightZoneDepth ?
            ((int)Submarine.Instance.GetSubmarineDepth() >= midnightZoneDepth ? _midnightOffset : _twiglightOffset) : _sunsetOffset;

        int newThreshold = ((int)Submarine.Instance.GetSubmarineDepth() / offset) * offset;
        int currDepth = (int)Submarine.Instance.GetSubmarineDepth();

        int sign = currDepth > previousDepth ? 1 : -1;
        SetNewDepth(sign, offset, currDepth);
        previousDepth = currDepth;

        /*if (newThreshold != previousThreshold)
        {
            
            previousThreshold = newThreshold;
            ChangeThreshold(offset);
        }*/
        //Debug.Log(Vector2.Distance(depthTransform.anchoredPosition, basePos));
        if (Vector2.Distance(depthTransform.anchoredPosition, basePos)>=29f)
        {
            previousThreshold = newThreshold;
            ChangeThreshold(offset);
        }
    }

    void ChangeThreshold(int offset)
    {
        for (int i = 0; i < 5; i++)
        {
            depthMeters[i].text = (previousThreshold + (i - 2) * offset).ToString();
        }

        depthTransform.anchoredPosition = basePos; 
    }

    void SetNewDepth(int sign, int offset, float depth)
    {
        float value = Mathf.Clamp01(Mathf.Abs(previousDepth - depth) / offset);
        targetPos = depthTransform.anchoredPosition + new Vector2(0, value*sign * cellHeight);
        depthTransform.anchoredPosition = targetPos; 
        //depthTransform.anchoredPosition = Vector2.Lerp(depthTransform.anchoredPosition, targetPos, Time.deltaTime * smoothSpeed);
    }

    private void DepthCheck()
    {
        depthTransform.anchoredPosition = basePos; 
        int offset = (int)Submarine.Instance.GetSubmarineDepth() >= twiglightZoneDepth ? 
            ((int)Submarine.Instance.GetSubmarineDepth() >=midnightZoneDepth? _midnightOffset: _twiglightOffset) : _sunsetOffset; 
        int currDepth = ((int)Submarine.Instance.GetSubmarineDepth() / offset) * offset;

        if (currDepth != previousThreshold) 
        {
            int sign = currDepth > previousThreshold ? 1 : -1;
            previousThreshold = currDepth;
            StartCoroutine(ChangeDepth(sign, offset));
        }
    }

    IEnumerator ChangeDepth(int sign, int offset)
    {
        
        Vector2 targetPos = basePos + new Vector2(0, sign*cellHeight);

        float elapsedTime = 0;
        while (elapsedTime < rollDuration)
        {
            depthTransform.anchoredPosition = Vector2.Lerp(basePos, targetPos, elapsedTime / rollDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        for (int i = 0; i < 5; i++)
        {
            depthMeters[i].text = (previousThreshold + (i - 2) * offset).ToString();
        }

        depthTransform.anchoredPosition = basePos;
    }
}
