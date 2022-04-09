using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mining : MonoBehaviour
{
    public float hitRadius = 3f;
    public Transform cameraPosition;
    public ChunkLoader chunkLoader;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {

            RaycastHit[] hits = Physics.RaycastAll(cameraPosition.position, cameraPosition.rotation * (new Vector3(0,0,1)), hitRadius);
            if (hits.Length == 0) return;
            RaycastHit frontHit = hits[0];

            Vector3 blockApprox = (frontHit.point - 0.5f * frontHit.normal);
            Vector3Int blockCollided = new Vector3Int(
                Mathf.FloorToInt(blockApprox.x + 0.5f),
                Mathf.FloorToInt(blockApprox.y + 0.5f),
                Mathf.FloorToInt(blockApprox.z + 0.5f)
            );
            chunkLoader.RemoveBlock(blockCollided);
        }
        if (Input.GetButtonDown("Fire2"))
        {

            RaycastHit[] hits = Physics.RaycastAll(cameraPosition.position, cameraPosition.rotation * (new Vector3(0, 0, 1)), hitRadius);
            if (hits.Length == 0) return;
            RaycastHit frontHit = hits[0];

            Vector3 blockApprox = (frontHit.point + 0.5f * frontHit.normal);
            Vector3Int blockCollided = new Vector3Int(
                Mathf.FloorToInt(blockApprox.x + 0.5f),
                Mathf.FloorToInt(blockApprox.y + 0.5f),
                Mathf.FloorToInt(blockApprox.z + 0.5f)
            );
            chunkLoader.AddBlock(blockCollided);
        }
    }
}
