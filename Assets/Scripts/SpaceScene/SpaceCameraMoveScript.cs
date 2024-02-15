using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static CartoonFX.CFXR_Effect;

public class SpaceCameraMoveScript : MonoBehaviour
{
    private Vector3[] vectors = new Vector3[5];
    private Quaternion[] rotates = new Quaternion[5];

    private Vector3 gameoverVec = Vector3.zero;
    private Quaternion gameoverRot = Quaternion.identity;

    private int currentIndex = 1;
    private Camera mainCamera;
    //카메라 드래그
    public Transform target; // The target the camera looks at (the origin)
    public float radius = 10.0f; // Distance from the target
    private float theta = Mathf.PI / 4; // Initial polar angle in radians
    private float phi = Mathf.PI / 4; // Initial azimuthal angle in radians
    private Vector2 lastTouchPosition;
    private float sensitivity = 0.01f; // Adjust this value to change rotation sensitivity
    private Vector3 lastMousePosition;
    [SerializeField]
    private SpaceTongsMoveScript spaceTongsMove;
    //카메라 드래그
    public int GetCurrentCameraIndex()
    {
        return currentIndex;
    }
    void Start()
    {
        mainCamera = Camera.main;

        vectors[0] = new Vector3(0, 15f, 0);  // Set position
        rotates[0] = Quaternion.Euler(90, 0, 0);  // Set rotation

        vectors[1] = new Vector3(0, 10, -10);  // Set position
        rotates[1] = Quaternion.Euler(30, 0, 0);  // Set rotation

        vectors[2] = new Vector3(-10, 10, 0);
        rotates[2] = Quaternion.Euler(30, 90, 0);

        vectors[3] = new Vector3(0, 10, 10);
        rotates[3] = Quaternion.Euler(30, 180, 0);

        vectors[4] = new Vector3(10, 10, 0);
        rotates[4] = Quaternion.Euler(30, 270, 0);

        gameoverVec = new Vector3(12, 2.0f, 0);
        gameoverRot = Quaternion.Euler(0, 270, 0);

        mainCamera.transform.position = vectors[currentIndex];
        mainCamera.transform.rotation = rotates[currentIndex];
    }
    private void Update()
    {
        //if (!spaceTongsMove.IsTongsMoving())
        //{ 
        //    if (Input.touchCount > 0)
        //    {
        //        Touch touch = Input.GetTouch(0);

        //        if (touch.phase == TouchPhase.Moved)
        //        {
        //            Vector2 delta = touch.deltaPosition;

        //            // Update phi and theta based on touch movement
        //            // Clamp theta to prevent the camera from going below the horizon (Y=0)
        //            phi -= delta.x * sensitivity;
        //            theta = Mathf.Clamp(theta - delta.y * sensitivity, 0.01f, Mathf.PI / 2);

        //            UpdateCameraPosition();
        //        }
        //    }
        //}

        if (!spaceTongsMove.IsTongsMoving())
        {
            if (Input.GetMouseButton(0))
            {
                Vector3 delta = Input.mousePosition - lastMousePosition;
                phi -= delta.x * sensitivity;
                theta = Mathf.Clamp(theta - delta.y * sensitivity, 0.01f, Mathf.PI / 2);
                UpdateCameraPosition();

            }
            lastMousePosition = Input.mousePosition;
        }
       


    }
    void UpdateCameraPosition()
    {
        // Convert spherical to Cartesian coordinates
        float x = radius * Mathf.Sin(theta) * Mathf.Cos(phi);
        float y = radius * Mathf.Cos(theta);
        float z = radius * Mathf.Sin(theta) * Mathf.Sin(phi);

        // Update camera position and rotation
        transform.position = new Vector3(x, y, z) + target.position;
        transform.LookAt(target);
    }

    public void MoveToNextPosition()
    {
        if (vectors.Length == 0 || rotates.Length == 0)
            return;
        currentIndex = (currentIndex + 1) % vectors.Length;
        // Set camera position and rotation
        mainCamera.transform.position = vectors[currentIndex];
        mainCamera.transform.rotation = rotates[currentIndex];

    }

    public void GameOverCameraMove()
    {
        mainCamera.transform.position = gameoverVec;
        mainCamera.transform.rotation = gameoverRot;
    }
    public void TriggerCameraShake()
    {
        
        float duration = 0.2f;
        float magnitude = 0.1f;
        StartCoroutine(CameraShake(duration, magnitude));
    }
    public IEnumerator CameraShake(float duration, float magnitude)
    {
        
        Vector3 originalPos = mainCamera.transform.position;
        Quaternion originalRot = mainCamera.transform.rotation;
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;


            mainCamera.transform.localPosition = new Vector3(originalPos.x + x, originalPos.y + y, originalPos.z);

            elapsed += Time.deltaTime;
            yield return null;
        }

        mainCamera.transform.position = originalPos;
        mainCamera.transform.rotation = originalRot;
    }
}
