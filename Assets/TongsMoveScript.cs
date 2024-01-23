using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class TongsMoveScript : MonoBehaviour
{
    private bool _isMoving = true;
    private float moveSpeed = 4.0f;
    private float minX, maxX, minZ, maxZ;
    private Rigidbody rb;
    private SpherePrefabScript heldSphere = null;

    //랜덤 시스템관련
    private System.Random random = new System.Random();
    //랜덤 시스템 관련

    // Start is called before the first frame update
    void Start()
    {
        minX = -1.9f;
        maxX = 1.9f;
        minZ = -1.9f;
        maxZ = 1.9f;
        rb = GetComponent<Rigidbody>();

        FirstSettingSphereMove();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && heldSphere != null)
        {
            float offsetX = (float)(random.NextDouble() * 0.02 - 0.01); // Random value between -0.01 and 0.01
            float offsetZ = (float)(random.NextDouble() * 0.02 - 0.01); // Random value between -0.01 and 0.01
            Vector3 newPosition = this.transform.position + new Vector3(offsetX, 0, offsetZ);
            heldSphere.transform.position = newPosition;

            heldSphere.SetTarget(null);
            Rigidbody sphereRb = heldSphere.GetComponent<Rigidbody>();
            if (sphereRb != null)
            {
                sphereRb.isKinematic = false; // Enable gravity (if Rigidbody is used)
            }
            heldSphere = null; // Clear the reference
        }
    }
    void FixedUpdate()
    {
        if (_isMoving)
        {
            float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");

            if (x != 0 || z != 0)
            {
                Vector3 movement = new Vector3(x, 0, z) * moveSpeed * Time.deltaTime;
                // Apply movement
                transform.Translate(movement, Space.World);

                // Clamp position
                float clampedX = Mathf.Clamp(transform.position.x, minX, maxX);
                float clampedZ = Mathf.Clamp(transform.position.z, minZ, maxZ);
               

                transform.position = new Vector3(clampedX, transform.position.y, clampedZ);
            }
        }
    }

    public void SettingSphereMove()
    {
        if (heldSphere == null) 
        {
            int type = GetRandomNumber();
            SpherePrefabScript sphere = GameManager.Instance.GetObject(type);
            HoldSphere(sphere);
        }
    }
    public void FirstSettingSphereMove()//첫번째만 쓰이는 용도
    {
        if (heldSphere == null)
        {
            SpherePrefabScript sphere = GameManager.Instance.GetObject(0);
            HoldSphere(sphere);
        }
    }

    public void HoldSphere(SpherePrefabScript sphere)
    {
        if (heldSphere != null)
        {
            // Optionally, release the currently held sphere
            // Similar to the spacebar logic
        }

        heldSphere = sphere;
        heldSphere.SetTarget(this.transform);
        Rigidbody sphereRb = heldSphere.GetComponent<Rigidbody>();
        if (sphereRb != null)
        {
            sphereRb.isKinematic = true; // Disable gravity (if Rigidbody is used)
        }
    }


    public int GetRandomNumber()//점수에 따른 스폰
    {
        if (GameManager.Instance.Score >= 2000)//2천점 이상일 때 
        {
            int randomNumber = random.Next(0, 100); // Random number between 0 and 99

            if (randomNumber < 30) // 30% probability for 1
            {
                return 1;
            }
            else if (randomNumber < 60) // Additional 30% probability for 2 (30% + 30%)
            {
                return 2;
            }
            else if (randomNumber < 80) // Additional 20% probability for 3 (30% + 30% + 20%)
            {
                return 3;
            }
            else if (randomNumber < 90) // Additional 10% probability for 4 (30% + 30% + 20% + 10%)
            {
                return 4;
            }
            else // Remaining 10%
            {
                return 0;
            }
        }
        else //2천점 미만 
        {
            int randomNumber = random.Next(0, 100); // Random number between 0 and 99

            if (randomNumber < 40) // 0 나올 확률 40% 
            {
                return 0;
            }
            else if (randomNumber < 70) // 30% probability (40% + 30%) 1나올 확률 30%
            {
                return 1;
            }
            else if (randomNumber < 90) // 20% probability (40% + 30% + 20%) 2나올 확률 20%
            {
                return 2;
            }
            else if (randomNumber < 98) // 8% probability (40% + 30% + 20% + 8%) 3나올 확률 8% 
            {
                return 3;
            }
            else // Remaining 2% 4나올 확률 2%
            {
                return 4;
            }
        }
        
    }
}
