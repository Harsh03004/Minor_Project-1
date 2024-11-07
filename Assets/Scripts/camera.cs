using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camera : MonoBehaviour
{
    public Transform player;  // Reference to the player's transform 
    public float zoomLevel = 5f;  // Variable to adjust the zoom
    public Vector3 offset = new Vector3(0, 1, -5);  // Offset for the camera

    private Camera cam;
    private Vector3 originalPosition;
    private float shakeDuration = 0.1f;  // Duration of shake
    private float shakeMagnitude = 0.05f;  // Magnitude of shake
    private float shakeTime = 0f;

    void Start()
    {
        cam = Camera.main;  // Get the camera component
        cam.orthographicSize = zoomLevel;  // Set the initial zoom level
    }

    // Update is called once per frame
    void Update()
    {
        // Follow the player
        Vector3 targetPosition = player.position + offset;

        // Trigger shake if duration is active
        if (shakeTime > 0)
        {
            targetPosition += Random.insideUnitSphere * shakeMagnitude;
            shakeTime -= Time.deltaTime;
        }

        // Set the camera position
        transform.position = targetPosition;

        // Adjust the camera zoom based on the zoomLevel variable
        cam.orthographicSize = zoomLevel;

        // Reset shake time if not shaking
        if (shakeTime <= 0)
        {
            shakeTime = 0f;  // Reset shake time
        }

        // Check for input to trigger camera shake (e.g., mouse button click)
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))  // 0 is the left mouse button
        {
            TriggerCameraShake(shakeDuration, shakeMagnitude);  // Trigger shake
        }
    }

    public void TriggerCameraShake(float duration, float magnitude)
    {
        shakeDuration = duration;
        shakeMagnitude = magnitude;
        shakeTime = duration;  // Set the shake time
    }
}
