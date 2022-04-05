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
        float capFactor = (persistance - 1)/(Mathf.Pow(persistance, octaves) - 1);
        float[,] noiseMap = Noise.GenerateNoiseMap(mapWidth, mapHeight, noiseScale, octaves, persistance, lacunarity, capFactor, offsetX, offsetY);
        int[,] heightMap = noise2height(noiseMap, factor);

        MapDisplay display = FindObjectOfType<MapDisplay>();
        
        display.DrawNoiseMapAsCubeMesh(heightMap);
        display.DrawTexture();
    }

    private int[,] noise2height(float[,] noiseMap, float factor)
    {
        int width = noiseMap.GetLength(0);
        int height = noiseMap.GetLength(1);
        int[,] heightMap = new int[width, height];
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                heightMap[i, j] = (int)(noiseMap[i, j] * factor);
            }
        }
        return heightMap;
    }
}
