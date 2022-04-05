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

    Dictionary<Vector2Int, Chunk> chunks;
    int oldx, oldy;

    void Start()
    {
        chunks = new Dictionary<Vector2Int, Chunk>();

        int x = ((int)playerPosition.transform.position.x) / chunkWidth;
        int y = ((int)playerPosition.transform.position.z) / chunkDepth;
        Vector2Int chunkPosition = new Vector2Int(x, y);

        Chunk chunk = null;
        if (chunks.ContainsKey(chunkPosition))
        {
            chunk = chunks[chunkPosition];
        }
        else
        {
            chunk = newChunk(x, y);
        }

        GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        plane.name = "Chunk " + x + " " + y;

        MapDisplay mapDisplay = plane.AddComponent(typeof(MapDisplay)) as MapDisplay;

        mapDisplay.Init();
        mapDisplay.AddTextureAtlas();
        mapDisplay.DrawMeshFromBlocks(chunk);

        plane.transform.Translate(new Vector3(16f * x, 0, 16f * y));

        oldx = x;
        oldy = y;
    }

    private void Update()
    {
        int x = ((int)playerPosition.transform.position.x) / chunkWidth;
        int y = ((int)playerPosition.transform.position.z) / chunkDepth;
        if (oldx == x && oldy == y) return;
        Debug.Log(x + " " + y);

        Vector2Int chunkPosition = new Vector2Int(x, y);

        Chunk chunk = null;
        if (chunks.ContainsKey(chunkPosition))
        {
            chunk = chunks[chunkPosition];
        }
        else
        {
            chunk = newChunk(x, y);
        }

        GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        plane.name = "Chunk " + x + " " + y;

        MapDisplay mapDisplay = plane.AddComponent(typeof(MapDisplay)) as MapDisplay;

        mapDisplay.Init();
        mapDisplay.AddTextureAtlas();
        mapDisplay.DrawMeshFromBlocks(chunk);

        plane.transform.Translate(new Vector3(16f * x, 0, 16f * y));

        oldx = x;
        oldy = y;
    }

    public void ZeroChunk()
    {
        Chunk chunk = newChunk(0, 0);

        GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        plane.AddComponent(typeof(MapDisplay));

        MapDisplay mapDisplay = plane.GetComponent<MapDisplay>();

        mapDisplay.Init();
        mapDisplay.AddTextureAtlas();
        mapDisplay.DrawMeshFromBlocks(chunk);
    }


    private Chunk newChunk(float offsetX, float offsetY)
    {
        int[,] heightMap = Noise.noise2height(
            Noise.GenerateNoiseMap(chunkWidth, chunkDepth, noiseScale, octaves, persistance, lacunarity, offsetX, offsetY),
            factor
        );

        Block[,,] chunkBlocks = new Block[chunkWidth, chunkDepth, chunkHeight];
        for (int i=0; i<chunkWidth; i++)
        {
            for (int j = 0; j < chunkDepth; j++)
            {
                for (int k = 0; k < chunkHeight; k++)
                {
                    if (k < heightMap[i, j]) chunkBlocks[i, j, k] = new Block(BlockType.Stone);
                    else chunkBlocks[i, j, k] = new Block(BlockType.Air);
                }
            }
        }

        return new Chunk(chunkBlocks);
    }
}
