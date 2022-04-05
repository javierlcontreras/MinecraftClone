using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class Noise
{
    public static int[,] noise2height(float[,] noiseMap, float factor)
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

    public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, float scale,
                                            int octaves, float persistance, float lacunarity,
                                            float offsetX, float offsetY)
    {
        float capFactor = (persistance - 1) / (Mathf.Pow(persistance, octaves) - 1);
        float[,] noiseMap = new float[mapWidth, mapHeight];
        for (int i=0; i<mapWidth; i++)
        {
            for (int j=0; j<mapHeight; j++)
            {
                float noiseHeight = 0;
                float frequency = 1;
                float amplitude = 1;

                for (int k=0; k<octaves; k++)
                {

                    float sampleX = 1.0f * i / scale * frequency;
                    float sampleY = 1.0f * j / scale * frequency;

                    float value = Mathf.PerlinNoise(sampleX + offsetX, sampleY + offsetY);
                    noiseHeight += value * amplitude;

                    frequency *= lacunarity;
                    amplitude *= persistance;
                }

                noiseMap[i, j] = noiseHeight * capFactor;
            }
        }
        return noiseMap;
    }
}
