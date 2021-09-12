using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    Camera cam;
    CharacterController cc;

    public float moveSpeed = 2;
    public float lookSpeed = 5;


    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
        cc = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.IsGameActive)
        {
            Vector3 moveInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));

            Vector3 move = transform.TransformDirection(moveInput.normalized);

            // Move the player
            cc.Move(move * moveSpeed * Time.deltaTime);


            // Change Look direction
            Vector2 lookInput = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

            Vector3 lookAngle = transform.eulerAngles;
            lookAngle.x = cam.transform.localEulerAngles.x;

            lookAngle.y += lookInput.x * lookSpeed * 10 * Time.deltaTime;
            lookAngle.x += -lookInput.y * lookSpeed * 10 * Time.deltaTime;

            // Limit how far player cn look up
            if (lookAngle.x > 85 && lookAngle.x < 100)
            {
                lookAngle.x = 85;
            }
            // Limit how far player can look down
            if (lookAngle.x < 175 && lookAngle.x > 150)
            {
                lookAngle.x = 175;
            }

            transform.eulerAngles = new Vector3(0, lookAngle.y, 0);
            cam.transform.localEulerAngles = new Vector3(lookAngle.x, 0, 0);
        }
    }
}
