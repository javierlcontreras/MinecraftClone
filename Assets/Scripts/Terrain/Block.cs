using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block 
{
    public BlockType type;
    public Block(BlockType _type)
    {
        type = _type;
    }
}

public enum BlockType
{
    Grass, Dirt, Stone, Air
}
