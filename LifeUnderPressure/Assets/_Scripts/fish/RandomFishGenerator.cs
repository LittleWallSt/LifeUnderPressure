using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RandomFishGenerator : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private GameObject fish;
    [SerializeField] private Path path;
    [SerializeField] private int nFish;
    [SerializeField] private float spawnDistance;
    [SerializeField] private float distance;

    private Transform player;

    private Queue<GameObject> fishList = new Queue<GameObject>();

    private bool spawned;
    
    void Start()
    {
        if (Submarine.Instance != null)
            player = Submarine.Instance.transform;
        else
            player = this.transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (!spawned)
        {
            if (Vector3.Distance(transform.position, player.position) < distance)
            {
                for (int i = 0; i < nFish; i++)
                {
                    Vector3 newPosition = transform.position + Random.insideUnitSphere * spawnDistance;
                    GameObject newFish = Instantiate(fish, newPosition, Random.rotation);
                    newFish.GetComponent<Fish>().SetPath(path);
                    newFish.GetComponent<Fish>().SetRandomWaypoint();
                    fishList.Enqueue(newFish);
                }
                spawned = true;
            }
        }
        else if (spawned && Vector3.Distance(transform.position, player.position) > distance) {
            for (int i = 0; i < nFish; i++)
            {
                GameObject fish = fishList.Dequeue();
                Destroy(fish);
            }
            spawned = false;
        }
        
    }
}
