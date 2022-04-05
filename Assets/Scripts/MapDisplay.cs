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

    public void DrawNoiseMap(int[,] heightMap, int factor)
    {
        int width = heightMap.GetLength(0);
        int height = heightMap.GetLength(1);
        Texture2D texture = new Texture2D(width, height)
        {
            filterMode = FilterMode.Point
        };

        Color[] textureColors = new Color[width * height];
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                float c = 1.0f * heightMap[i, j]/factor;
                textureColors[i + j * width] = new Color(c, c, c);
            }
        }

        texture.SetPixels(textureColors);
        texture.Apply();

        meshRenderer.sharedMaterial.mainTexture = texture;
    }


    private int coord2value(int x, int y, int w, int h) {
        return x * h + y;
    }

    public void DrawTexture()
    {
        Texture2D texture = null;
        byte[] fileData;
        
        if (File.Exists(texturePath))
        {
            fileData = File.ReadAllBytes(texturePath);
            texture = new Texture2D(2, 2);
            texture.filterMode = FilterMode.Point;
            texture.LoadImage(fileData); //..this will auto-resize the texture dimensions.
            texture.Apply();
            Debug.Log("EO");

        }
        Debug.Log("OMG " + texturePath);

        meshRenderer.sharedMaterial.mainTexture = texture;
    }

    public void DrawNoiseMapAsMesh(float[,] noiseMap)
    {
        int width = noiseMap.GetLength(0);
        int height = noiseMap.GetLength(1);
        
        int[] triangles = new int[(width-1)*(height-1)*6]; // meshFilter.mesh.triangles;
        Vector3[] vertices = new Vector3[width * height];
        Vector2[] uv = new Vector2[width * height];

        for (int i = 0; i+1 < width; i++)
        {
            for (int j = 0; j+1 < height; j++)
            {
                int ind = 6 * coord2value(i,j,width-1, height-1);
                triangles[ind]     = coord2value(i, j, width, height);
                triangles[ind + 1] = coord2value(i + 1, j + 1, width, height);
                triangles[ind + 2] = coord2value(i+1, j, width, height);

                triangles[ind + 3] = coord2value(i, j, width, height);
                triangles[ind + 4] = coord2value(i, j + 1, width, height);
                triangles[ind + 5] = coord2value(i+1, j+1, width, height);
            }
        }


        for (int i=0; i<width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                int ind = coord2value(i, j, width, height);
                Debug.Log(noiseMap[i, j]);
                vertices[ind] = new Vector3(i, 255*noiseMap[i,j], j);
                uv[ind] = new Vector2(1f*i/width, 1f*j/height);
            }
        }

        meshFilter.sharedMesh.vertices = vertices;
        meshFilter.sharedMesh.uv = uv;
        meshFilter.sharedMesh.triangles = triangles;
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


    private Vector2[] divideBySize(Vector2[] uvs, int width, int height) {
        int si = uvs.Length;
        for (int i = 0; i < si; i++) {
            uvs[i].Scale(new Vector2(1f/width, 1f / height));
        }
        return uvs;
    }

}