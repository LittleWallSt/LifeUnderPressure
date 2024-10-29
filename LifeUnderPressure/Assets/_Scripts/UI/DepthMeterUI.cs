using System.Collections;
using TMPro;
using UnityEngine;

public class DepthMeterUI : MonoBehaviour
{
    [SerializeField]TextMeshProUGUI[] depthMeters;
    [SerializeField]RectTransform depthTransform;

    [SerializeField] float cellHeight = 30f;
    [SerializeField] float rollDuration = 0.2f;
    [SerializeField] int _offset = 50;

    [SerializeField] float midnightZoneDepth = 900f;
    [SerializeField] int _midnightOffset = 150;

    int previousDepth;

    Vector2 basePos;

    private void Start()
    {
        previousDepth = ((int)Submarine.Instance.GetSubmarineDepth()/_offset)*_offset;
        basePos = depthTransform.anchoredPosition;
    }

    private void Update()
    {
        DepthCheck();
    }

    public void DepthCheck()
    {
        
        depthTransform.anchoredPosition = basePos; 
        int offset = (int)Submarine.Instance.GetSubmarineDepth() >= midnightZoneDepth ? _midnightOffset : _offset;
        int currDepth = ((int)Submarine.Instance.GetSubmarineDepth() / offset) * offset;

        if (currDepth != previousDepth) 
        {
            int sign = currDepth > previousDepth ? 1 : -1;
            previousDepth = currDepth;
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
            depthMeters[i].text = (previousDepth + (i - 2) * offset).ToString();
        }

        depthTransform.anchoredPosition = basePos;
    }
}
