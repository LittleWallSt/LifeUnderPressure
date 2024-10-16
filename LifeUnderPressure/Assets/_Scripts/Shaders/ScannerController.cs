using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScannerController : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] float speed;
    [SerializeField] float delayDestroyTime;


    void Start()
    {
        DestroyEffect();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 vectorMesh = this.transform.localScale;
        float growing = this.speed*Time.deltaTime;
        this.transform.localScale = new Vector3(vectorMesh.x + growing, vectorMesh.y + growing, vectorMesh.z + growing);   

    }

    private void DestroyEffect()
    {
        Destroy(this.gameObject, delayDestroyTime);
    }
}
