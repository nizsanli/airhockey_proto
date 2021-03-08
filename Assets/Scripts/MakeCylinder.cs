using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[RequireComponent(typeof(MeshCollider))]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class MakeCylinder : MonoBehaviour
{
    MeshRenderer meshRenderer;
    MeshFilter meshFilter;
    MeshCollider meshCollider;

    public int numSides;
    public float radius;
    public float height;

    private void OnValidate()
    {
        CreateMesh();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void CreateMesh()
    {
        meshRenderer = gameObject.GetComponent<MeshRenderer>();
        meshFilter = gameObject.GetComponent<MeshFilter>();
        meshCollider = gameObject.GetComponent<MeshCollider>();

        Vector3[] vertices = new Vector3[(numSides + 1) * 2];
        int[] indices = new int[((numSides) * 2 + (numSides) * 2) * 3];

        // middle and first
        vertices[0] = Vector3.zero;
        vertices[1] = Vector3.up * height;
        vertices[2] = Vector3.right * radius;
        vertices[3] = Vector3.right * radius + Vector3.up * height;

        float sideAngle = 2f * Mathf.PI / numSides;
        for (int k = 1; k < numSides; k++)
        {
            Vector3 offset = new Vector3(Mathf.Cos(k * sideAngle), 0f, Mathf.Sin(k * sideAngle)) * radius;

            vertices[(k + 1) * 2] = offset;
            vertices[(k + 1) * 2 + 1] = offset + Vector3.up * height;
        }

        for (int k = 0; k < numSides; k++)
        {
            int s = 12 * k;

            if (k < numSides - 1)
            {
                // bottom tri
                indices[s] = 0;
                indices[s + 1] = (k + 1) * 2;
                indices[s + 2] = (k + 2) * 2;

                // top tri
                indices[s + 3] = 1;
                indices[s + 4] = (k + 2) * 2 + 1;
                indices[s + 5] = (k + 1) * 2 + 1;

                // side tris
                indices[s + 6] = (k + 1) * 2;
                indices[s + 7] = (k + 1) * 2 + 1;
                indices[s + 8] = (k + 2) * 2;

                indices[s + 9] = (k + 1) * 2 + 1;
                indices[s + 10] = (k + 2) * 2 + 1;
                indices[s + 11] = (k + 2) * 2;
            }
            else
            {
                // bottom tri
                indices[s] = 0;
                indices[s + 1] = (k + 1) * 2;
                indices[s + 2] = 2;

                // top tri
                indices[s + 3] = 1;
                indices[s + 4] = 3;
                indices[s + 5] = (k + 1) * 2 + 1;

                // side tris
                indices[s + 6] = (k + 1) * 2;
                indices[s + 7] = (k + 1) * 2 + 1;
                indices[s + 8] = 2;

                indices[s + 9] = (k + 1) * 2 + 1;
                indices[s + 10] = 3;
                indices[s + 11] = 2;
            }
        }

        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = indices;
        mesh.Optimize();

        meshFilter.sharedMesh = mesh;

        meshRenderer.sharedMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit"));
    }

    [ContextMenu("SaveMesh")]
    public void SaveMesh()
    {
        Mesh meshToSave = Instantiate<Mesh>(meshFilter.sharedMesh);

        MeshUtility.Optimize(meshToSave);
        AssetDatabase.CreateAsset(meshToSave, "Assets/CylinderMesh.asset");
        AssetDatabase.SaveAssets();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
