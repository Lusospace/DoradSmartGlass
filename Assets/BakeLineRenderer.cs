using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BakeLineRenderer : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject lineObj;
    public Material GreenMat;
    void Start()
    {

        BakeLineDebuger(lineObj, GreenMat);
    }
    public static void BakeLineDebuger(GameObject lineObj, Material GreenMat)
    {
        var lineRenderer = lineObj.GetComponent<LineRenderer>();
        var meshFilter = lineObj.AddComponent<MeshFilter>();
        Mesh mesh = new Mesh();
        lineRenderer.BakeMesh(mesh);
        meshFilter.sharedMesh = mesh;

        var meshRenderer = lineObj.AddComponent<MeshRenderer>();
        meshRenderer.sharedMaterial = GreenMat;

        GameObject.Destroy(lineRenderer);

        // Move the mesh object to its parent's location
        lineObj.transform.position = lineObj.transform.parent.position;
    }
}
