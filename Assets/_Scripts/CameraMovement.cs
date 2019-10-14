using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public Transform cameraPosition;
    public Camera camera;

    public float speed = 10;
    public float shiftMultiplier = 2;
    public float camSens = 0.25f;

    
    private float distance = 1f;
    private Vector3 lastMouse;
    private Vector3 GetBaseInput()
    {
        Vector3 velocity = new Vector3();
        if (Input.GetKey(KeyCode.W)) {
            velocity += Vector3.forward;
        }
        if (Input.GetKey(KeyCode.S)) {
            velocity += Vector3.back;
        }
        if (Input.GetKey(KeyCode.A)) {
            velocity += Vector3.left;
        }
        if (Input.GetKey(KeyCode.D)) {
            velocity += Vector3.right;
        }
        return velocity;
    }

    void Update() 
    {
        if (Input.mouseScrollDelta.y != 0) {
            distance = Mathf.Clamp(distance - Input.mouseScrollDelta.y / 3, 0.2f, 50);
        }

        Vector3 input = GetBaseInput() * speed;
        if (Input.GetKey(KeyCode.LeftShift)) {
            input *= shiftMultiplier;
        }
        input *= Time.deltaTime;
        
        Vector3 newPosition = transform.position;
        transform.Translate(input);
        newPosition.x = transform.position.x;
        newPosition.z = transform.position.z;
        transform.position = newPosition;
    }
}
