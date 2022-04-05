using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk
{
    public Block[,,] blocks;
    int[,] heightMap;

    public Chunk(Block[,,] _blocks)
    {
        blocks = _blocks;
    }
}
