using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishHabitat : MonoBehaviour, IDistanceLoad
{
    [SerializeField] private Fish fishPrefab = null;
    [SerializeField] private int maxAmount = 3;
    [SerializeField] private float zoneRadius = 10f;
    [SerializeField, Range(0.1f, 1.0f)] private float minScale;
    [SerializeField, Range(1.0f, 2.0f)] private float maxScale;
    [SerializeField] private bool randomPath = false;
    
    // Javi >>
    [SerializeField] private Path path = null;
    // << Javi

    private bool working = false;

    private List<Fish> fishList = new List<Fish>();
    private void Start()
    {
        IDL_AssignToGameManager();
        for(int i = 0; i < maxAmount; i++)
        {
            int checks = 0;
            Vector3 spawnPos = transform.position + Random.insideUnitSphere * zoneRadius;
            Collider[] colls = Physics.OverlapSphere(spawnPos, 0.2f, InternalSettings.EnvironmentLayer);

            float terrainHeight = GameManager.Instance.GetTerrainHeight(spawnPos);
            if(spawnPos.y < terrainHeight) spawnPos.y = terrainHeight + 0.2f;

            while(colls.Length > 0 && checks < 50)
            {
                spawnPos = transform.position + Random.insideUnitSphere * zoneRadius;
                colls = Physics.OverlapSphere(spawnPos, 0.2f, InternalSettings.EnvironmentLayer);
                checks++;
            }

            fishList.Add(Instantiate(fishPrefab, spawnPos, Quaternion.identity));
            // Javi >>
            fishList[i].SetPath(path);
            fishList[i].gameObject.transform.localScale *= Random.Range(minScale, maxScale);
            if(randomPath)
                fishList[i].GetComponent<Fish>().SetRandomPath();
            // << Javi
        }
    }

    // IDL calls
    public void IDL_OffDistance()
    {
        if (working)
        {
            working = false;
            foreach(Fish fish in fishList)
            {
                fish.gameObject.SetActive(false);
            }
        }
    }

    public void IDL_InDistance()
    {
        if (!working)
        {
            working = true;
            foreach (Fish fish in fishList)
            {
                fish.gameObject.SetActive(true);
            }
        }
    }
    public void IDL_AssignToGameManager()
    {
        GameManager.Instance.AssignIDL(this);
    }

    public Vector3 IDL_GetPosition()
    {
        return transform.position;
    }
    // Gizmo
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 0, 0, 0.3f);
        Gizmos.DrawSphere(transform.position, zoneRadius);
    }
}
