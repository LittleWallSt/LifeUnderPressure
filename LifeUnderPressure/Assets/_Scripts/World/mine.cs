using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Collides, explodes, not much more to it
/// Spawns a particle system on death
/// </summary>
public class mine : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> explosionVFX = new List<GameObject>();
    
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == 6) {
            Submarine.Instance.DamageSubmarine(1000, DamageType.Mine);
            this.gameObject.SetActive(false);

            for (int i = 0; i < explosionVFX.Count; i++) {
                explosionVFX[i].SetActive(true);
                explosionVFX[i].GetComponent<ParticleSystem>().Play();
            }

            Destroy(gameObject);
        }
    }
}
