using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CameraMoveScript : MonoBehaviour
{
    private Vector3[] vectors = new Vector3[4];
    private Quaternion[] rotates = new Quaternion[4];

    private Vector3 gameoverVec = Vector3.zero;
    private Quaternion gameoverRot = Quaternion.identity;

    private int currentIndex = 0;

    void Start()
    {
        vectors[0] = new Vector3(0,10,-10);  // Set position
        rotates[0] = Quaternion.Euler(30,0,0);  // Set rotation

        vectors[1] = new Vector3(-10, 10, 0);  
        rotates[1] = Quaternion.Euler(30, 90, 0);

        vectors[2] = new Vector3(0, 10, 10);  
        rotates[2] = Quaternion.Euler(30, 180, 0);

        vectors[3] = new Vector3(10, 10, 0);  
        rotates[3] = Quaternion.Euler(30, 270, 0);

        gameoverVec = new Vector3(12, 2.5f, 0);
        gameoverRot = Quaternion.Euler(0, 270, 0);
    }

    public  void MoveToNextPosition()
    {
        if (vectors.Length == 0 || rotates.Length == 0)
            return;

        // Set camera position and rotation
        Camera.main.transform.position = vectors[currentIndex];
        Camera.main.transform.rotation = rotates[currentIndex];
        currentIndex = (currentIndex + 1) % vectors.Length;
    }

    public void GameOverCameraMove() 
    {
        Camera.main.transform.position = gameoverVec;
        Camera.main.transform.rotation = gameoverRot;
    }

}
