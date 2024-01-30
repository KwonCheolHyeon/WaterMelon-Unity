using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SlimeTongsMoveScript : MonoBehaviour
{
    private bool _isMoving = true;
    private float moveSpeed = 4.0f;
    private float minX, maxX, minZ, maxZ;

    private SlimePrefabScript heldSlime = null;

    //연속 떨구는거 딜레이 주기 위한 bool값
    private bool isReleasing = false;

    //랜덤 시스템관련
    private System.Random random = new System.Random();
    //랜덤 시스템 관련

    //라인 보여주는 함수
    private LineRenderer lineRenderer;
    public float rayLength = 10f;
    //라인을 보여주는 관련

    //조이스틱 관련
    public VariableJoystick variableJoystick;
    //조이스틱 관련

    //다음 나올 과일을 보여주는 관련
    [SerializeField]
    private TextMeshProUGUI textMeshProUGUI;
    private int nextTypeSlime;
    //다음 나올 과일을 보여주는 관련

    // Start is called before the first frame update
    void Start()
    {
        minX = -1.8f;
        maxX = 1.8f;
        minZ = -1.8f;
        maxZ = 1.8f;

        lineRenderer = GetComponent<LineRenderer>();
        FirstSettingSphereMove();
    }

    private void Update()
    {
        if (lineRenderer != null)
        {
            lineRenderer.enabled = (heldSlime != null);
        }

        if (Input.GetKeyDown(KeyCode.Space) && heldSlime != null && !isReleasing)
        {
            StartCoroutine(ReleaseSlimeWithDelay());
        }

        if (lineRenderer != null && lineRenderer.enabled)
        {
            DrawTrajectory();
        }

    }
    void FixedUpdate()
    {
        if (_isMoving && Camera.main != null)
        {
            float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");
            x += variableJoystick.Horizontal;
            z += variableJoystick.Vertical;

            if (x != 0 || z != 0)
            {
                Vector3 forward = Camera.main.transform.forward;
                Vector3 right = Camera.main.transform.right;
                forward.y = 0; // Keep the movement horizontal
                right.y = 0;
                forward.Normalize();
                right.Normalize();

                Vector3 movement = (forward * z + right * x) * moveSpeed * Time.deltaTime;

                // Apply movement
                transform.Translate(movement, Space.World);

                // Clamp position
                float clampedX = Mathf.Clamp(transform.position.x, minX, maxX);
                float clampedZ = Mathf.Clamp(transform.position.z, minZ, maxZ);
                transform.position = new Vector3(clampedX, transform.position.y, clampedZ);
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject != null && heldSlime != null && collision.gameObject != heldSlime.gameObject)
        {
            SlimeGameManager.Instance.GameOver();
        }
    }


    private IEnumerator SettingSphereMoveWithDelay()
    {
        yield return new WaitForSeconds(0.2f); // Delay of 0.2 seconds

        SlimePrefabScript sphere = SlimeGameManager.Instance.GetObject(nextTypeSlime);
        HoldSlime(sphere);

    }
    private void SettingSphereMove()
    {
        if (heldSlime == null)
        {
            SlimePrefabScript sphere = SlimeGameManager.Instance.GetObject(nextTypeSlime);
            HoldSlime(sphere);
        }
        else
        {
            SlimeGameManager.Instance.GameOver();
        }
    }

    public void FirstSettingSphereMove()//첫번째만 쓰이는 용도
    {
        if (heldSlime == null)
        {
            SlimePrefabScript sphere = SlimeGameManager.Instance.GetObject(0);
            HoldSlime(sphere);
        }
    }

    // DropButton.OnClick에서 참조
    public void DropSlime()
    {
        if (heldSlime != null)
        {
            float offsetX = (float)(random.NextDouble() * 0.02 - 0.01); // Random value between -0.01 and 0.01
            float offsetZ = (float)(random.NextDouble() * 0.02 - 0.01); // Random value between -0.01 and 0.01
            Vector3 newPosition = this.transform.position + new Vector3(offsetX, 0, offsetZ);
            heldSlime.transform.position = newPosition;

            heldSlime.SetTarget(null);
            Rigidbody rb = heldSlime.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false; // Enable gravity (if Rigidbody is used)
            }

            MeshCollider sphereCollider = heldSlime.GetComponent<MeshCollider>();
            if (sphereCollider != null)
            {
                sphereCollider.enabled = true;
            }

            heldSlime = null; // Clear the reference
        }
    }

    public void HoldSlime(SlimePrefabScript _slime)
    {
        if (heldSlime != null)
        {
            // Optionally, release the currently held sphere
            // Similar to the spacebar logic
        }

        heldSlime = _slime;
        heldSlime.SetTarget(this.transform);
        Rigidbody rb = heldSlime.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true; // Disable gravity (if Rigidbody is used)
        }
    }


    public int GetRandomNumber()//점수에 따른 스폰
    {
        if (SlimeGameManager.Instance.Score >= 2000)//2천점 이상일 때 
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

    void DrawTrajectory()
    {
        Vector3 start = transform.position;
        Vector3 direction = Vector3.down;
        lineRenderer.SetPosition(0, start);

        if (Physics.Raycast(start, direction, out RaycastHit hit, rayLength))
        {
            SetLineRendererGradientAtPoint(hit.distance / rayLength);
            lineRenderer.SetPosition(1, hit.point);
        }
        else
        {
            ResetLineRendererGradient();
            lineRenderer.SetPosition(1, start + direction * rayLength);
        }
    }

    void SetLineRendererGradientAtPoint(float relativePoint)
    {
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(Color.white, 0.0f), new GradientColorKey(Color.red, relativePoint), new GradientColorKey(Color.red, 1.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(1.0f, relativePoint), new GradientAlphaKey(1.0f, 1.0f) }
        );
        lineRenderer.colorGradient = gradient;
    }

    void ResetLineRendererGradient()
    {
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(Color.white, 0.0f), new GradientColorKey(Color.white, 1.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(1.0f, 1.0f) }
        );
        lineRenderer.colorGradient = gradient;
    }

    public void NextSphererInforMation() // 다음 구체에 대한 정보
    {
        SettingSphereMove();

        nextTypeSlime = GetRandomNumber();
        textMeshProUGUI.text = nextTypeSlime + ": Next";
    }

    private IEnumerator ReleaseSlimeWithDelay()
    {
        isReleasing = true;
        yield return new WaitForSeconds(0.1f); // Delay of 0.2 seconds

        // Existing logic for releasing the sphere
        float offsetX = (float)(random.NextDouble() * 0.02 - 0.01);
        float offsetZ = (float)(random.NextDouble() * 0.02 - 0.01);
        Vector3 newPosition = this.transform.position + new Vector3(offsetX, 0, offsetZ);
        heldSlime.transform.position = newPosition;

        heldSlime.SetTarget(null);
        Rigidbody rb = heldSlime.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
        }
        MeshCollider meshColl = heldSlime.GetComponent<MeshCollider>();
        if (meshColl != null)
        {
            meshColl.enabled = true;
        }

        heldSlime = null;

        isReleasing = false; // Reset the flag
    }
}
