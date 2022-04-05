using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMover : MonoBehaviour
{
    public CharacterController controller;

    private Vector3 playerVelocity;
    public float playerSpeed = 6.0f;
    public float jumpHeight = 0.7f;
    public float gravityValue = -9.81f;

    private float sensibility = 6;

    private float yRotation;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensibility;
        yRotation += mouseX;

        Vector3 orig = controller.transform.position - new Vector3(0, 0.45f, 0);
        Vector3 north = new Vector3(0,0,1);
        Vector3 east = new Vector3(1,0,0);

        bool groundedPlayer = Physics.Raycast(orig, Vector3.down, 0.35f);
        bool cappedNorth = Physics.Raycast(orig, north, 0.35f);
        bool cappedSouth = Physics.Raycast(orig, -north, 0.35f);
        bool cappedEast = Physics.Raycast(orig, east, 0.35f);
        bool cappedWest = Physics.Raycast(orig, -east, 0.35f);

        //Debug.Log(orig + " " + north + " " + east);
        //Debug.Log(groundedPlayer + " " + cappedNorth + " " + cappedSouth + " " + cappedEast + " " + cappedWest);
        //Debug.Log(jumpHeight);

        if (/*groundedPlayer && */Input.GetButtonDown("Jump"))
        {
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
        }

        playerVelocity.y += gravityValue * Time.deltaTime;
        
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }
        
        Vector3 move = controller.transform.localToWorldMatrix * (new Vector3(2*Input.GetAxis("Horizontal"), 0f, 2*Input.GetAxis("Vertical")));

        if (cappedNorth && move.z > 0) move.z = 0;
        if (cappedSouth && move.z < 0) move.z = 0;
        if (cappedEast && move.x > 0) move.x = 0;
        if (cappedWest && move.x < 0) move.x = 0;

        if (playerVelocity.y != 0)
        {
            controller.transform.Translate(playerVelocity * Time.deltaTime);
        }
        if (move != Vector3.zero)
        {
            controller.transform.Translate(move * Time.deltaTime * playerSpeed, Space.World);
        }
        controller.transform.localRotation = Quaternion.Euler(0, yRotation, 0);

    }
}