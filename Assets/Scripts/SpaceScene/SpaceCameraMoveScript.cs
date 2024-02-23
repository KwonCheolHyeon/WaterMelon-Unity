using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
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
    public Transform target;
    private float radius;
    private float theta = Mathf.PI / 4; 
    private float phi = Mathf.PI / 4; 
    private Vector2 lastTouchPosition;
    private float sensitivity;
    private Vector3 lastMousePosition;
    [SerializeField]
    private SpaceTongsMoveScript spaceTongsMove;
    //카메라 드래그
    //게임 오버
    private bool isGameOver = false;

    [SerializeField]
    private Slider cameraRadius;
    [SerializeField]
    private Slider cameraSensitivity;

    public int GetCurrentCameraIndex()
    {
        return currentIndex;
    }
    void Start()
    {
        mainCamera = Camera.main;

        gameoverVec = new Vector3(12, 2.0f, 0);
        gameoverRot = Quaternion.Euler(0, 270, 0);

        sensitivity = 0.002f;
        radius = 12.0f;

        cameraSensitivity.value = sensitivity;
        cameraRadius.value = radius;

        UpdateCameraPosition();
    }
    private void Update()
    {
        sensitivity = cameraSensitivity.value;
        radius = cameraRadius.value;

        // 터치한 부분이 UI일 경우 true 반환
        if (EventSystem.current.IsPointerOverGameObject() == false)
        {
#if UNITY_EDITOR
            if (!spaceTongsMove.IsTongsMoving() && !isGameOver && Input.GetMouseButton(0))
            {
                Vector3 delta = Input.mousePosition - lastMousePosition;
                phi -= delta.x * sensitivity;
                theta = Mathf.Clamp(theta - delta.y * sensitivity, 0.01f, Mathf.PI / 2);
            }
#else
        if (!spaceTongsMove.IsTongsMoving() && !isGameOver)
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0); // 첫 번째 터치
                float adjustedSensitivity = sensitivity * (Screen.width / 1080);
                switch (touch.phase)
                {
                    case TouchPhase.Began:
                        lastTouchPosition = touch.position;
                        break;

                    case TouchPhase.Moved:
                        Vector2 touchDelta = touch.deltaPosition;
                        // 정규화된 터치 이동 거리를 사용하여 카메라 회전 각도 조정
                        phi -= touchDelta.x * adjustedSensitivity;
                        theta = Mathf.Clamp(theta - touchDelta.y * adjustedSensitivity, 0.01f, Mathf.PI / 2);
                        break;
                }
            }
        }
#endif
        }
        UpdateCameraPosition();
        lastMousePosition = Input.mousePosition;
    }

    void UpdateCameraPosition()
    {
        
        float x = radius * Mathf.Sin(theta) * Mathf.Cos(phi);
        float y = radius * Mathf.Cos(theta);
        float z = radius * Mathf.Sin(theta) * Mathf.Sin(phi);

        
        transform.position = new Vector3(x, y, z) + target.position;
        transform.LookAt(target);
    }

    public void MoveToNextPosition()
    {
        sensitivity *= 0.1f;
        if (vectors.Length == 0 || rotates.Length == 0)
            return;
        currentIndex = (currentIndex + 1) % vectors.Length;

        mainCamera.transform.position = vectors[currentIndex];
        mainCamera.transform.rotation = rotates[currentIndex];

    }

    public void GameOverCameraMove()
    {
        isGameOver = true;
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
