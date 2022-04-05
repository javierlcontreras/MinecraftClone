using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public int mapWidth;
    public int mapHeight;
    public float noiseScale;
    public int octaves;
    public float lacunarity;
    [Range(0,1)]
    public float persistance;
    public float offsetX;
    public float offsetY;
    public int factor;

    public bool autoUpdate;
    
    private void Start()
    {
        GenerateMap();
    }

    public void GenerateMap() {
        Chunk chunk = ChunkLoader.newChunk(0, 0);

        Debug.Log(chunk.blocks[12,8,0].type);

        MapDisplay display = FindObjectOfType<MapDisplay>();

        display.AddTextureAtlas();
        display.DrawMeshFromBlocks(chunk);
    }

}
