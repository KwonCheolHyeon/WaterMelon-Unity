using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CameraMoveScript : MonoBehaviour
{
    private Vector3[] vectors = new Vector3[5];
    private Quaternion[] rotates = new Quaternion[5];

    private Vector3 gameoverVec = Vector3.zero;
    private Quaternion gameoverRot = Quaternion.identity;

    private int currentIndex = 1;
    private Camera mainCamera;

    public int GetCurrentCameraIndex() 
    {
        return currentIndex;
    }
    void Start()
    {
        mainCamera = Camera.main;

        vectors[0] = new Vector3(0, 9.2f, 0);  // Set position
        rotates[0] = Quaternion.Euler(90, 0, 0);  // Set rotation

        vectors[1] = new Vector3(0,10,-10);  // Set position
        rotates[1] = Quaternion.Euler(30,0,0);  // Set rotation

        vectors[2] = new Vector3(-10, 10, 0);  
        rotates[2] = Quaternion.Euler(30, 90, 0);

        vectors[3] = new Vector3(0, 10, 10);  
        rotates[3] = Quaternion.Euler(30, 180, 0);

        vectors[4] = new Vector3(10, 10, 0);  
        rotates[4] = Quaternion.Euler(30, 270, 0);

        gameoverVec = new Vector3(12, 2.5f, 0);
        gameoverRot = Quaternion.Euler(0, 270, 0);

        mainCamera.transform.position = vectors[currentIndex];
        mainCamera.transform.rotation = rotates[currentIndex];
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public  void MoveToNextPosition()
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
