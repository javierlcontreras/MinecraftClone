using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class Noise
{
    public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, float scale,
                                            int octaves, float persistance, float lacunarity, float capFactor,
                                            float offsetX, float offsetY)
    {
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
