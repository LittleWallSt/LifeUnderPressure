using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishHabitat : MonoBehaviour, IDistanceLoad
{
    [SerializeField] private Fish fishPrefab = null;
    [SerializeField] private int maxAmount = 3;
    [SerializeField] private float zoneRadius = 10f;
    
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
                Debug.Log("cc");
                spawnPos = transform.position + Random.insideUnitSphere * zoneRadius;
                colls = Physics.OverlapSphere(spawnPos, 0.2f, InternalSettings.EnvironmentLayer);
                checks++;
            }

            fishList.Add(Instantiate(fishPrefab, spawnPos, Quaternion.identity));
            // Javi >>
            fishList[i].GetComponent<Fish>().SetPath(path);
            // << Javi
        }
    }

    // IDL calls
    public void IDL_OffDistance()
    {
        if (working)
        {
            working = false;
            Debug.Log("off dista");
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
            Debug.Log("in  dista");
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
