using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class CameraMoveScript : MonoBehaviour
{
    private CameraSettingDatas cameraDatas;
    private string datakey = "CameraSettingDatas";
    private string saveFileName = "SaveCameraSettingFile.es3";

    private Vector3[] vectors = new Vector3[5];
    private Quaternion[] rotates = new Quaternion[5];

    private Vector3 gameoverVec = Vector3.zero;
    private Quaternion gameoverRot = Quaternion.identity;

    private int currentIndex = 1;
    private Camera mainCamera;
    //ī�޶� �巡��
    public Transform target;
    private Vector2 lastTouchPosition;
    private Vector3 lastMousePosition;
    private float theta = Mathf.PI / 4;
    private float phi = Mathf.PI / 4;

    [SerializeField]
    private SlimeTongsMoveScript slimeTongsMove;

    //���� ����
    private bool isGameOver = false;

    [SerializeField]
    private UnityEngine.UI.Slider cameraRadius;
    [SerializeField]
    private UnityEngine.UI.Slider cameraSensitivity;


    public int GetCurrentCameraIndex() 
    {
        return currentIndex;
    }
    void Start()
    {
        mainCamera = Camera.main;
       
        gameoverVec = new Vector3(0, 1.8f, -12);
        gameoverRot = Quaternion.Euler(0, 0, 0);

        DataLoad();

        cameraSensitivity.value = cameraDatas.sensitivity;
        cameraRadius.value = cameraDatas.radius;

        UpdateCameraPosition();
    }

    private void Update()
    {
        if (cameraSensitivity.value != cameraDatas.sensitivity || cameraRadius.value != cameraDatas.radius)
        {
            CameraDataSave();
        }
        cameraDatas.sensitivity = cameraSensitivity.value;
        cameraDatas.radius = cameraRadius.value;
#if UNITY_EDITOR
        // ��ġ�� �κ��� UI�� ��� true ��ȯ
        if (EventSystem.current.IsPointerOverGameObject(-1) == false)
        {
            if (!slimeTongsMove.IsTongsMoving() && !isGameOver && Input.GetMouseButton(0))
            {
                Vector3 delta = Input.mousePosition - lastMousePosition;
                phi -= delta.x * cameraDatas.sensitivity;
                theta = Mathf.Clamp(theta - delta.y * cameraDatas.sensitivity, 0.01f, Mathf.PI / 2);
            }
        }
#else
    // ��ġ�� �κ��� UI�� ��� true ��ȯ
    if (EventSystem.current.IsPointerOverGameObject(0) == false)
    {
        if (!slimeTongsMove.IsTongsMoving() && !isGameOver)
        {
            if (Input.touchCount == 2) // �� �հ������� ��ġ�ϴ� ���
            {
                Touch touchZero = Input.GetTouch(0);
                Touch touchOne = Input.GetTouch(1);

                Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
                Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

                float prevMagnitude = (touchZeroPrevPos - touchOnePrevPos).magnitude;
                float currentMagnitude = (touchZero.position - touchOne.position).magnitude;

                float difference = currentMagnitude - prevMagnitude;

                Zoom(difference * 0.01f); // �� �� �� �� �ƿ��� ���� ���� ��
            }
            else if (Input.touchCount == 1) // �� �հ������� ��ġ�ϴ� ���
            {
                Touch touch = Input.GetTouch(0);
                float adjustedSensitivity = cameraDatas.sensitivity * (Screen.width / 1080);
                switch (touch.phase)
                {
                    case TouchPhase.Began:
                        lastTouchPosition = touch.position;
                        break;

                    case TouchPhase.Moved:
                        Vector2 touchDelta = touch.deltaPosition;
                        // ����ȭ�� ��ġ �̵� �Ÿ��� ����Ͽ� ī�޶� ȸ�� ���� ����
                        phi -= touchDelta.x * adjustedSensitivity;
                        theta = Mathf.Clamp(theta - touchDelta.y * adjustedSensitivity, 0.01f, Mathf.PI / 2);
                        break;
                }
            }
        }
    }
#endif
        // radius�� ���� ���� �ٷιٷ� ����
        UpdateCameraPosition();
        lastMousePosition = Input.mousePosition;
    }

    void Zoom(float increment)
    {
        cameraRadius.value = Mathf.Clamp(cameraDatas.radius - increment, cameraRadius.minValue, cameraRadius.maxValue); // �ּ� �� �ִ� �� �� ����
    }

    void UpdateCameraPosition()
    {
        float x = cameraDatas.radius * Mathf.Sin(theta) * Mathf.Cos(phi);
        float y = cameraDatas.radius * Mathf.Cos(theta);
        float z = cameraDatas.radius * Mathf.Sin(theta) * Mathf.Sin(phi);

        transform.position = new Vector3(x, y, z) + target.position;
        transform.LookAt(target);
    }


    public void MoveToNextPosition()
    {
        cameraDatas.sensitivity *= 0.1f;
        if (vectors.Length == 0 || rotates.Length == 0)
            return;
        currentIndex = (currentIndex + 1) % vectors.Length;
        // Set camera position and rotation
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

    public void DataLoad()
    {
        if (ES3.FileExists(saveFileName) && ES3.KeyExists(datakey, saveFileName))
        {

            cameraDatas = ES3.Load<CameraSettingDatas>(datakey, saveFileName);
            cameraRadius.value = cameraDatas.radius;
            cameraSensitivity.value = cameraDatas.sensitivity;
        }
        else
        {
            cameraDatas = new CameraSettingDatas();
            CameraDataSave();

            cameraDatas = ES3.Load<CameraSettingDatas>(datakey, saveFileName);
            Debug.Log("ī�޶� ���� ������ �ε� �Ϸ�");
        }
    }

    public void CameraDataSave()
    {
        cameraDatas.radius = cameraRadius.value;
        cameraDatas.sensitivity = cameraSensitivity.value;

        ES3.Save(datakey, cameraDatas, saveFileName);

        Debug.Log("ī�޶� ���� ������ ���� �Ϸ�");
    }
}
