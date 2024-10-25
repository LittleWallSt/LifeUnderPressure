using System.Collections;
using TMPro;
using UnityEngine;

public class DepthMeterUI : MonoBehaviour
{
    [SerializeField]TextMeshProUGUI[] depthMeters;
    [SerializeField]RectTransform depthTransform;

    [SerializeField] float cellHeight = 30f;
    [SerializeField] float rollDuration = 0.5f;
    [SerializeField] int offset = 50;

    int previousDepth;

    private void Start()
    {
        previousDepth = ((int)Submarine.Instance.GetSubmarineDepth()/50)*50;

    }

    private void Update()
    {
        DepthCheck();
    }

    public void DepthCheck()
    {
        int currDepth = ((int)Submarine.Instance.GetSubmarineDepth() / 50) * 50;
        if (currDepth != previousDepth)
        {
            int sign = currDepth > previousDepth ? 1 : -1;
            previousDepth = currDepth;
            StartCoroutine(ChangeDepth(sign));
        }
    }

    IEnumerator ChangeDepth(int sign)
    {
        Vector2 basePos = depthTransform.anchoredPosition;
        Vector2 targetPos = basePos + new Vector2(0, sign*30);

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
