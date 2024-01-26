using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CameraMoveScript : MonoBehaviour
{
    private Camera camera;

    private Vector3[] vectors = new Vector3[4];
    private Quaternion[] rotates = new Quaternion[4];
    private int currentIndex = 0;

    void Start()
    {
        camera = GetComponent<Camera>();

        vectors[0] = new Vector3(0,10,-10);  // Set position
        rotates[0] = Quaternion.Euler(30,0,0);  // Set rotation

        vectors[1] = new Vector3(-10, 10, 0);  
        rotates[1] = Quaternion.Euler(30, 90, 0);

        vectors[2] = new Vector3(0, 10, 10);  
        rotates[2] = Quaternion.Euler(30, 180, 0);

        vectors[3] = new Vector3(10, 10, 0);  
        rotates[3] = Quaternion.Euler(30, 270, 0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public  void MoveToNextPosition()
    {
        if (vectors.Length == 0 || rotates.Length == 0)
            return;

        // Set camera position and rotation
        camera.transform.position = vectors[currentIndex];
        camera.transform.rotation = rotates[currentIndex];
        currentIndex = (currentIndex + 1) % vectors.Length;
    }

}
