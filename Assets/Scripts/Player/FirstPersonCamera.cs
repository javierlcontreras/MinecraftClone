using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonCamera : MonoBehaviour
{
    public Transform cameraPosition;
    public Transform playerPosition;
    private float xRotation;
    private float sensibility = 6;
    void Start()
    {

    }

    void Update()
    {
        float mouseY = Input.GetAxis("Mouse Y") * sensibility;

        cameraPosition.position = playerPosition.position + new Vector3(0,1f,0);
        cameraPosition.rotation = playerPosition.rotation;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -80, 80);
        cameraPosition.rotation = Quaternion.Euler(xRotation, cameraPosition.rotation.eulerAngles.y, 0);
    }
}
