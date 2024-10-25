using UnityEngine;

public class Compass : MonoBehaviour
{
    [SerializeField] Transform submarine;
    [SerializeField]
    Transform[] letters;
    Vector3 dir;

    void Start()
    {
        if (submarine == null) submarine = Submarine.Instance.transform;
    }

    void Update()
    {
        dir.z = submarine.eulerAngles.y;
        transform.localEulerAngles = dir; 

        foreach (Transform t in letters)
        {
            t.localEulerAngles = -dir;
        }
    }
}
