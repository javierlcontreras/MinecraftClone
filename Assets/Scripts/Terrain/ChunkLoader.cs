using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkLoader : MonoBehaviour
{
    private const int chunkWidth = 16;
    private const int chunkDepth = 16;
    private const int chunkHeight = 50;
    public float noiseScale = 16;
    public int octaves = 5;
    public float lacunarity = 3.68f;
    [Range(0, 1)]
    public float persistance = 0.26f;
    [Range(0, 255)]
    public int factor = 10;

    public Transform playerPosition;
    public int loadingRadius = 3;

    Dictionary<Vector2Int, Chunk> chunks;
    private int oldx, oldy;
    private bool start;
    private GameObject chunkParent;
    void Start()
    {
        start = true;
        chunkParent = GameObject.FindGameObjectWithTag("ChunkParent");
        chunks = new Dictionary<Vector2Int, Chunk>();
    }

    private void Update()
    {
        int x = Mathf.FloorToInt(playerPosition.transform.position.x / chunkWidth);
        int y = Mathf.FloorToInt(playerPosition.transform.position.z / chunkDepth);
        if (!start && oldx == x && oldy == y) return;
        
        Vector2Int chunkPosition = new Vector2Int(x, y);
        Vector2Int oldChunkPosition = new Vector2Int(oldx, oldy);

        for (int i=x-loadingRadius; i<=x+loadingRadius; i++)
        {
            for (int j = y-loadingRadius; j <= y+loadingRadius; j++)
            {
                Vector2Int thisChunk = new Vector2Int(i, j);
                bool loadedBefore = !start && (Vector2.Distance(thisChunk, oldChunkPosition) < loadingRadius);
                bool loadedNow = (Vector2.Distance(thisChunk, chunkPosition) < loadingRadius);

                //Debug.Log(x + " " + y + " " + i + " " + j + " " + loadedBefore + " " + loadedNow);

                if (loadedBefore && !loadedNow)
                {
                    if (chunks.ContainsKey(thisChunk))
                    {
                        chunks[thisChunk].plane.SetActive(false);
                    }
                }
                if (!loadedBefore && loadedNow)
                {
                    if (!chunks.ContainsKey(thisChunk))
                    {
                        Chunk chunk = newChunk(i, j);

                        GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
                        plane.name = "Chunk " + i + " " + j;

                        MapDisplay mapDisplay = plane.AddComponent(typeof(MapDisplay)) as MapDisplay;

                        mapDisplay.Init();
                        mapDisplay.AddTextureAtlas();
                        mapDisplay.DrawMeshFromBlocks(chunk);

                        plane.transform.SetParent(chunkParent.transform);
                        plane.transform.Translate(new Vector3(chunkWidth * i, 0, chunkDepth * j));
                        chunk.plane = plane;
                        
                        chunks[thisChunk] = chunk;
                    }

                    chunks[thisChunk].plane.SetActive(true);
                }
            }
        }

        start = false;
        oldx = x;
        oldy = y;
    }
    List<GameObject> testingPlanes;
    public void ZeroChunk()
    {
        testingPlanes = new List<GameObject>();
        for (int i=-loadingRadius; i<=loadingRadius; i++)
        {
            for (int j = -loadingRadius; j <= loadingRadius; j++)
            {
                Chunk chunk = newChunk(0, 0);

                GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
                testingPlanes.Add(plane);
                plane.AddComponent(typeof(MapDisplay));
                
                MapDisplay mapDisplay = plane.GetComponent<MapDisplay>();

                mapDisplay.Init();
                mapDisplay.AddTextureAtlas();
                mapDisplay.DrawMeshFromBlocks(chunk);
            }
        }
    }

    public void Reload()
    {
        start = true;
        Debug.Log("JAVI");
        foreach (Chunk chunk in chunks.Values)
        {
            GameObject.DestroyImmediate(chunk.plane);
        }
        chunks.Clear();
    }

    public void CleanUp()
    {
        testingPlanes.ForEach((GameObject x) => GameObject.DestroyImmediate(x));
    }


    private Chunk newChunk(float chunkX, float chunkY)
    {
        int[,] heightMap = Noise.noise2height(
            Noise.GenerateNoiseMap(chunkWidth, chunkDepth, noiseScale, octaves, persistance, lacunarity, chunkWidth * chunkX, chunkDepth * chunkY),
            factor
        );

        Block[,,] chunkBlocks = new Block[chunkWidth, chunkHeight, chunkDepth];
        for (int i=0; i<chunkWidth; i++)
        {
            for (int j = 0; j < chunkDepth; j++)
            {
                for (int k = 0; k < chunkHeight; k++)
                {
                    if (k == heightMap[i, j]) chunkBlocks[i, k, j] = new Block(BlockType.Grass);
                    else if (k < heightMap[i, j]) chunkBlocks[i, k, j] = new Block(BlockType.Stone);
                    else chunkBlocks[i, k, j] = new Block(BlockType.Air);
                }
            }
        }

        return new Chunk(chunkBlocks);
    }

    public void RemoveBlock(Vector3Int block)
    {
        int chunkX = Mathf.FloorToInt(1f * block.x / chunkWidth);
        int chunkZ = Mathf.FloorToInt(1f * block.z / chunkDepth);
        Vector2Int chunkPosition = new Vector2Int(chunkX, chunkZ);
        Chunk chunk = chunks[chunkPosition];
        block.x = ((block.x % 16) + 16) % 16;
        block.z = ((block.z % 16) + 16) % 16;

        //Debug.Log(block);
        //Debug.Log(chunk.blocks[block.x, block.y, block.z].type);
        chunk.blocks[block.x, block.y, block.z].type = BlockType.Air;
        chunk.Reload();
    }

    public void AddBlock(Vector3Int block)
    {
        int chunkX = Mathf.FloorToInt(1f * block.x / chunkWidth);
        int chunkZ = Mathf.FloorToInt(1f * block.z / chunkDepth);
        Vector2Int chunkPosition = new Vector2Int(chunkX, chunkZ);
        Chunk chunk = chunks[chunkPosition];
        block.x = ((block.x % 16) + 16) % 16;
        block.z = ((block.z % 16) + 16) % 16;

        //Debug.Log(block);
        //Debug.Log(chunk.blocks[block.x, block.y, block.z].type);
        chunk.blocks[block.x, block.y, block.z].type = BlockType.Grass;
        chunk.Reload();
    }
}
