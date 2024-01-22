using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class TongsMoveScript : MonoBehaviour
{
    private bool _isMoving = true;
    public float moveSpeed = 1.0f;
    private float minX, maxX, minZ, maxZ;
    private Rigidbody rb;
    private SpherePrefabScript heldSphere = null;

    // Start is called before the first frame update
    void Start()
    {
        minX = -1.9f;
        maxX = 1.9f;
        minZ = -1.9f;
        maxZ = 1.9f;
        rb = GetComponent<Rigidbody>();

        SettingSphereMove();
    }

    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.Space) && heldSphere != null)
        {
            // Release the sphere
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
}
