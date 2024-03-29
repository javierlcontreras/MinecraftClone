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

    Dictionary<string, Vector2[]> textureCoord;

    public void Init()
    {
        meshRenderer = gameObject.GetComponent<MeshRenderer>();
        meshCollider = gameObject.GetComponent<MeshCollider>();
        meshFilter = gameObject.GetComponent<MeshFilter>();
        texturePath = "Assets/Materials/TextureAtlas.png";
    }


    private Vector2[] TextureCoordsFromID(int textureIDx, int textureIDy, int atlasWidth, int atlasHeight)
    {
        float dx = 1f / atlasWidth;
        float dy = 1f / atlasHeight;

        float x = 1f * textureIDx / atlasWidth;
        float y = 1f * textureIDy / atlasHeight;
        return new Vector2[4] {
                    new Vector2(x, y-dy),
                    new Vector2(x+dx, y-dy),
                    new Vector2(x+dx, y),
                    new Vector2(x, y)
                };
    }

    public void AddTextureAtlas()
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
        
        textureCoord = new Dictionary<string, Vector2[]>();
        textureCoord.Add("grasstop", TextureCoordsFromID(0, 0, 2, 2));
        textureCoord.Add("grassside", TextureCoordsFromID(1, 0, 2, 2));
        textureCoord.Add("dirt", TextureCoordsFromID(0, 1, 2, 2));
        textureCoord.Add("stone", TextureCoordsFromID(1, 1, 2, 2));

        meshRenderer.sharedMaterial.mainTexture = texture;
    }

    private void addQuad(Vector3[] coord, Vector2[] uvs, List<Vector3> vertices, List<Vector2> uv, List<int> triangles, ref int nver, bool reorder, bool growing)
    {

        if (reorder)
        {
            Swap(ref coord[0], ref coord[3]);
            Swap(ref coord[1], ref coord[2]);
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

        if (reorder)
        {
            Swap(ref coord[0], ref coord[3]);
            Swap(ref coord[1], ref coord[2]);
        }
        if ((reorder && growing) || (!reorder && !growing))
        {
            Swap(ref uvs[0], ref uvs[3]);
            Swap(ref uvs[1], ref uvs[2]);
        }
    }

    public void DrawMeshFromBlocks(Chunk chunk)
    {
        //Debug.Log("EONOW");
        List<int> triangles = new List<int>();
        List<Vector3> vertices = new List<Vector3>();
        List<Vector2> uv = new List<Vector2>();
        int nver = 0;

        int chunkWidth = chunk.blocks.GetLength(0);
        int chunkkHeight = chunk.blocks.GetLength(1);
        int chunkDepth = chunk.blocks.GetLength(2);

        int[] neighboursX = new int[] { 1,-1,0,0,0,0};
        int[] neighboursZ = new int[] { 0,0,1,-1,0,0};
        int[] neighboursY = new int[] { 0,0,0,0,1,-1};

        for (int i=0; i<chunkWidth; i++)
        {
            for (int j = 0; j < chunkDepth; j++)
            {
                for (int k = 0; k < chunkkHeight; k++)
                {
                    BlockType blockType = chunk.blocks[i, k, j].type;
                    if (blockType == BlockType.Air) continue;

                    for (int neig = 0; neig < 6; neig++)
                    {
                        int x = i + neighboursX[neig];
                        int z = j + neighboursZ[neig];
                        int y = k + neighboursY[neig];
                        BlockType neighbourType = BlockType.Air;

                        if (0 <= x && x < chunkWidth &&
                            0 <= z && z < chunkDepth &&
                            0 <= y && y < chunkkHeight) neighbourType = chunk.blocks[x, y, z].type;

                        Vector2[] textureTop = textureCoord["grasstop"];
                        Vector2[] textureBot = textureCoord["dirt"];
                        Vector2[] textureSide = textureCoord["grassside"];
                        if (blockType == BlockType.Stone)
                        {
                            textureBot = textureTop = textureSide = textureCoord["stone"];
                        }

                        if (neighbourType == BlockType.Air)
                        {
                            if (k != y) { 
                                Vector3[] coord = new Vector3[4] {
                                    new Vector3(i-0.5f, (k+y)/2f, j-0.5f),
                                    new Vector3(i+0.5f, (k+y)/2f, j-0.5f),
                                    new Vector3(i+0.5f, (k+y)/2f, j+0.5f),
                                    new Vector3(i-0.5f, (k+y)/2f, j+0.5f)
                                };
                                if (k < y) addQuad(coord, textureTop, vertices, uv, triangles, ref nver, k > y, k > y);
                                else addQuad(coord, textureBot, vertices, uv, triangles, ref nver, k > y, k > y);
                            }
                            if (i != x)
                            {
                                Vector3[] coord = new Vector3[4] {
                                    new Vector3((i+x)/2f, k-0.5f, j-0.5f),
                                    new Vector3((i+x)/2f, k-0.5f, j+0.5f),
                                    new Vector3((i+x)/2f, k+0.5f, j+0.5f),
                                    new Vector3((i+x)/2f, k+0.5f, j-0.5f)
                                };
                                addQuad(coord, textureSide, vertices, uv, triangles, ref nver, i > x, true);
                            }
                            if (j != z)
                            {
                                Vector3[] coord = new Vector3[4] {
                                    new Vector3(i+0.5f, k-0.5f, (j+z)/2f),
                                    new Vector3(i-0.5f, k-0.5f, (j+z)/2f),
                                    new Vector3(i-0.5f, k+0.5f, (j+z)/2f),
                                    new Vector3(i+0.5f, k+0.5f, (j+z)/2f)
                                };
                                addQuad(coord, textureSide, vertices, uv, triangles, ref nver, j > z, true);
                            }
                        }
                    }
                }
            }
        }

        meshFilter.mesh.uv = null;
        meshFilter.mesh.triangles = null;

        meshFilter.mesh.vertices = vertices.ToArray();
        meshFilter.mesh.uv = uv.ToArray();
        meshFilter.mesh.triangles = triangles.ToArray();
        meshFilter.mesh.RecalculateNormals();

        meshCollider.sharedMesh = meshFilter.mesh;
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