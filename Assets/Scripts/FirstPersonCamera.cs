using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonCamera : MonoBehaviour
{
    public Transform cameraPosition;
    public Transform playerPosition;
    private float xRotation;
    private float sensibility = 6;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
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
