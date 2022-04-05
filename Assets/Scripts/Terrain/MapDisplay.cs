using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class MapDisplay : MonoBehaviour
{
    public Renderer meshRenderer;
    public MeshFilter meshFilter;
    public MeshCollider meshCollider;
    public string texturePath;

    public void DrawTexture()
    {
        Texture2D texture = null;
        byte[] fileData;
        
        if (File.Exists(texturePath))
        {
            fileData = File.ReadAllBytes(texturePath);
            texture = new Texture2D(2, 2);
            texture.filterMode = FilterMode.Point;
            texture.LoadImage(fileData);
            texture.Apply();
        }

        meshRenderer.sharedMaterial.mainTexture = texture;
    }

    private void addQuad(Vector3[] coord, Vector2[] uvs, List<Vector3> vertices, List<Vector2> uv, List<int> triangles, ref int nver, bool reorder, bool growing)
    {
        if (reorder)
        {
            Debug.Log(coord[0] + " " + coord[1] + " " + coord[2] + " " + coord[3]);
            Swap(ref coord[0], ref coord[3]);
            Swap(ref coord[1], ref coord[2]);
            Debug.Log(coord[0] + " " + coord[1] + " " + coord[2] + " " + coord[3]);

        }
        if ((reorder && growing) || (!reorder && !growing))
        {
            Swap(ref uvs[0], ref uvs[3]);
            Swap(ref uvs[1], ref uvs[2]);
        }

        vertices.AddRange(coord);
        uv.AddRange(uvs);
        triangles.AddRange(new int[] { nver, nver + 2, nver + 1, nver, nver + 3, nver + 2 });
        nver += 4;
    }

    public void DrawNoiseMapAsCubeMesh(int[,] heightMap) {
        List<int> triangles = new List<int>();
        List<Vector3> vertices = new List<Vector3>();
        List<Vector2> uv = new List<Vector2>();
        int nver = 0;

        int width = heightMap.GetLength(0);
        int height = heightMap.GetLength(1);
        int[] neighboursX = new int[2] { 1, 0};
        int[] neighboursY = new int[2] { 0, 1};

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++) 
            {
                int h = heightMap[i, j];

                Vector3[] coord = new Vector3[4] { 
                    new Vector3(i, h, j),
                    new Vector3(i + 1, h, j),
                    new Vector3(i + 1, h, j + 1),
                    new Vector3(i, h, j+1) 
                };
                Vector2[] uvs = new Vector2[4] {
                    new Vector2(0, 0.5f),
                    new Vector2(0.5f, 0.5f),
                    new Vector2(0.5f, 1f),
                    new Vector2(0, 1f)
                };

                addQuad(coord, uvs, vertices, uv, triangles, ref nver, false, false);


                for (int k = 0; k < 2; k++)
                {
                    int xn = i + neighboursX[k];
                    int yn = j + neighboursY[k];

                    if (xn < width && yn < height) {
                        int heightDiff = heightMap[xn, yn] - h;
                        if (heightDiff > 0)
                        {
                            for (int hit = 0; hit < heightDiff; hit++)
                            {

                                coord = new Vector3[4] {
                                    new Vector3(xn, h+hit, yn),
                                    new Vector3(i+1, h+hit, j + 1),
                                    new Vector3(i+1, h+hit+1, j + 1),
                                    new Vector3(xn, h+hit+1, yn)
                                };

                                uvs = new Vector2[4] {
                                    new Vector2(0.5f, 0.5f),
                                    new Vector2(1, 0.5f),
                                    new Vector2(1, 1),
                                    new Vector2(0.5f, 1)
                                };
                                addQuad(coord, uvs, vertices, uv, triangles, ref nver, k==0, true);
                            }
                        }
                        else if (heightDiff < 0)
                        {
                            for (int hit = 0; hit > heightDiff; hit--)
                            {
                                coord = new Vector3[4] {
                                    new Vector3(xn, h+hit, yn),
                                    new Vector3(i+1, h+hit, j + 1),
                                    new Vector3(i+1, h+hit-1, j + 1),
                                    new Vector3(xn, h+hit-1, yn)
                                };
                                uvs = new Vector2[4] {
                                    new Vector2(0.5f, 0.5f),
                                    new Vector2(1, 0.5f),
                                    new Vector2(1, 1),
                                    new Vector2(0.5f, 1)
                                };
                                addQuad(coord, uvs, vertices, uv, triangles, ref nver, k == 0, false);
                            }
                        }
                    }
                }
            }
        }

        meshFilter.sharedMesh.uv = null;
        meshFilter.sharedMesh.triangles = null;

        meshFilter.sharedMesh.vertices = vertices.ToArray();
        meshFilter.sharedMesh.uv = uv.ToArray();
        meshFilter.sharedMesh.triangles = triangles.ToArray();
        meshFilter.sharedMesh.RecalculateNormals();

        meshCollider.sharedMesh = meshFilter.sharedMesh;
    }

    private void Swap(ref Vector3 A, ref Vector3 B)
    {
        Vector3 C = B;
        B = A;
        A = C;
    }

    private void Swap(ref Vector2 A, ref Vector2 B)
    {
        Vector2 C = B;
        B = A;
        A = C;
    }
}