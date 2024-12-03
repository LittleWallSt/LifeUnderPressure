using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class LUPUtility
{

#if UNITY_EDITOR
    [MenuItem("LUP/Combine Selected Meshes")]
    public static void CombineSelectedMeshes()
    {
        Mesh mesh = new Mesh();
        List<CombineInstance> combines = new List<CombineInstance>();
        foreach(GameObject go in Selection.gameObjects)
        {
            MeshFilter mf = go.GetComponent<MeshFilter>();
            if (!mf || mf.sharedMesh == null) continue;

            CombineInstance combine = new CombineInstance()
            {
                mesh = mf.sharedMesh,
                transform = Matrix4x4.TRS(go.transform.position, go.transform.rotation,
                go.transform.localScale)
            };
            combines.Add(combine);
        }
        mesh.CombineMeshes(combines.ToArray());
        mesh.name = "Mesh_" + GUID.Generate();
        
        AssetDatabase.CreateAsset(mesh, "Assets/Models/Meshes/" + mesh.name + ".mesh");
    }
#endif
}
[Serializable]
public struct MovementVector
{
    public float forward;
    public float side;
    public float backward;
    public float upward;

    public MovementVector (float forward, float side, float backward, float upward)
    {
        this.forward = forward;
        this.side = side;
        this.backward = backward;
        this.upward = upward;
    }

}
[Serializable]
public struct ScannerStruct
{
    public float scanTimer;
    public float scanAnimationSpeed;
    public float depletingSpeed;
}
