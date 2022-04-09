using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk
{
    public Block[,,] blocks;
    public GameObject plane;
    
    public Chunk(Block[,,] _blocks)
    {
        blocks = _blocks;
    }

    public void Reload()
    {
        //Debug.Log("OK");
        MapDisplay mapDisplay = plane.GetComponent<MapDisplay>();
        mapDisplay.DrawMeshFromBlocks(this);
    }
}
