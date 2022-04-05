using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkLoader : MonoBehaviour
{
    private const int chunkWidth = 16;
    private const int chunkDepth = 16;
    private const int chunkHeight = 50;
    public static float noiseScale = 10;
    public static int octaves = 5;
    public static float lacunarity = 3.68f;
    [Range(0, 1)]
    public static float persistance = 0.26f;
    [Range(0, 255)]
    public static int factor = 10;

    public Transform playerPosition;

    void Start()
    {
        Chunk chunk = newChunk(0, 0); 
    }

    void Update()
    {
        
    }

    public static Chunk newChunk(float offsetX, float offsetY)
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
