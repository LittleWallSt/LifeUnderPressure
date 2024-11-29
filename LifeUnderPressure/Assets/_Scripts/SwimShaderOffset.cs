using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwimShaderOffset : MonoBehaviour
{
    [SerializeField] Material mat;
    [SerializeField] private MeshRenderer ren;

    // Start is called before the first frame update
    void Start()
    {
        Material mattie = Instantiate(mat);
        mattie.SetFloat("_Offset", Random.Range(0f,1f));
        ren.SetMaterials(new List<Material>(){mattie});
    }


}
