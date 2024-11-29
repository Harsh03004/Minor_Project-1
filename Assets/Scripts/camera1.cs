using System.Collections;
using UnityEngine;

public class camera1 : MonoBehaviour
{
    public Transform player;  // Reference to the player's transform 
    public float zoomLevel = 5f;  // Variable to adjust the zoom
    public Vector3 offset = new Vector3(0, 1, -5);  // Offset for the camera

    private Camera cam;
    private Vector3 originalPosition;
    private float shakeDuration = 0.1f;  // Duration of shake
    private float shakeMagnitude = 0.05f;  // Magnitude of shake
    private float shakeTime = 0f;

    public float leftBoundary = -8f;
    public float rightBoundary = 45f;
    public float lowerBoundary = -5f;
    public float upperBoundary = 20f;

    void Start()
    {
        cam = Camera.main;  // Get the camera component
        cam.orthographicSize = zoomLevel;  // Set the initial zoom level
    }

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

        // Reset shake time if not shaking
        if (shakeTime <= 0)
        {
            shakeTime = 0f;  // Reset shake time
        }

        // Adjust the camera zoom based on the zoomLevel variable
        cam.orthographicSize = zoomLevel;

        // Clamp the camera's position within the boundaries
        float cameraHalfHeight = cam.orthographicSize;
        float cameraHalfWidth = cam.aspect * cameraHalfHeight;

        float clampedX = Mathf.Clamp(targetPosition.x, leftBoundary + cameraHalfWidth, rightBoundary - cameraHalfWidth);
        float clampedY = Mathf.Clamp(targetPosition.y, lowerBoundary + cameraHalfHeight, upperBoundary - cameraHalfHeight);

        transform.position = new Vector3(clampedX, clampedY, targetPosition.z);

        // Check for input to trigger camera shake
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
