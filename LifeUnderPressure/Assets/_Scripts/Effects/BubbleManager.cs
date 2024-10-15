using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleManager : MonoBehaviour
{
    [SerializeField] private ParticleSystem bubblePrefab = null;
    [SerializeField] private float updateTime = 1f;
    [SerializeField] private int bubbleCount = 3;
    [SerializeField] private float bubbleSpawnDistance = 10f;
    [SerializeField] private float bubbleRenderDistance = 20f;

    private List<ParticleSystem> bubbles = new List<ParticleSystem>();

    private void Start()
    {
        for (int i = 0; i < bubbleCount; i++)
        {
            ParticleSystem bubble = Instantiate(bubblePrefab, transform);
            bubble.gameObject.SetActive(false);
            bubbles.Add(bubble);
        }
        StartCoroutine(BubblePSProcess());
    }
    private IEnumerator BubblePSProcess()
    {
        while (true)
        {
            Vector3 submarinePosition = Submarine.Instance.transform.position;
            foreach (ParticleSystem bubble in bubbles)
            {
                float distance = Vector3.Distance(bubble.transform.position, submarinePosition);
                if (distance > bubbleRenderDistance)
                {
                    Vector3 newBubblePosition = submarinePosition;
                    newBubblePosition += Random.insideUnitSphere * bubbleSpawnDistance;
                    newBubblePosition.y = GameManager.Instance.GetTerrainHeight(newBubblePosition);
                    bubble.transform.position = newBubblePosition;
                    bubble.gameObject.SetActive(true);
                }
            }
            yield return new WaitForSeconds(updateTime);
        }
    }
}
