using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpaceTongsMoveScript : MonoBehaviour
{
    private Camera mainCamera;
    private SpaceCameraMoveScript mCameraScript;
    private bool _isMoving = true;
    private float moveSpeed = 4.0f;
    private float[] sizeXYZ = new float [5];
    private float minX, maxX, minZ, maxZ;
    private Rigidbody _rigidbody;
    private Transform _transform;
    private SpaceSphereScript heldSphere = null;

    //연속 떨구는거 딜레이 주기 위한 bool값
    private bool isReleasing = false;
    private bool canDropSphere = true;
    //연속 떨구는거 딜레이 주기

    //랜덤 시스템관련
    private System.Random random = new System.Random();
    //랜덤 시스템 관련

    //라인 보여주는 함수
    private LineRenderer lineRenderer;
    public float rayLength = 10f;
    [SerializeField]
    private GameObject targetObject;
    //라인을 보여주는 관련

    //조이스틱 관련
    public VariableJoystick variableJoystick;
    private bool isTongsMoving = false;
    public bool IsTongsMoving() { return isTongsMoving; }
    //

    //단계별 행성 이미지
    [SerializeField]
    private List<Sprite> planetImageList = new List<Sprite>();

    //다음 나올 과일을 보여주는 관련
    [SerializeField]
    private Image nextPlanetImage;
    private int nextTypeSphere;
    private int nowTypeSphere;
    [SerializeField]
    private TextMeshProUGUI textChangeCount;
    private int changeCount;
    [SerializeField]
    private GameObject adPannel;
    public void SetChangeCount()
    {
        changeCount = 1;
        textChangeCount.text = changeCount + "";
    }

    void Start()
    {
        changeCount = 2;
        nextTypeSphere = 0;
        nowTypeSphere = nextTypeSphere;
        mainCamera = Camera.main;
        mCameraScript = mainCamera.GetComponent<SpaceCameraMoveScript>();
        _transform = transform;
        _rigidbody = GetComponent<Rigidbody>();
        sizeXYZ[0] = 1.9f;
        sizeXYZ[1] = 1.81f;
        sizeXYZ[2] = 1.72f;
        sizeXYZ[3] = 1.6f;
        sizeXYZ[4] = 1.5f;
        minX = -sizeXYZ[nowTypeSphere];
        maxX = sizeXYZ[nowTypeSphere];
        minZ = -sizeXYZ[nowTypeSphere];
        maxZ = sizeXYZ[nowTypeSphere];
        lineRenderer = GetComponent<LineRenderer>();
        FirstSettingSphereMove();
        //SettingSphereMove();
    }

    private void Update()
    {
        if (lineRenderer != null)
        {
            lineRenderer.enabled = (heldSphere != null);
            targetObject.SetActive((heldSphere != null));
        }

        if (Input.GetKeyDown(KeyCode.Space) && heldSphere != null && !isReleasing)
        {
            StartCoroutine(ReleaseSphereWithDelay());
        }

        if (lineRenderer != null && lineRenderer.enabled)
        {
            DrawTrajectory();
        }

    }
    void FixedUpdate()
    {
      

        if (_isMoving && mainCamera != null)
        {
            float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");
            x += variableJoystick.Horizontal;
            z += variableJoystick.Vertical;

            if (Mathf.Abs(x) > 0.01f || Mathf.Abs(z) > 0.01f)
            {
                isTongsMoving = true;
            }
            else
            {
                isTongsMoving = false;
            }

            minX = -sizeXYZ[nowTypeSphere];
            maxX = sizeXYZ[nowTypeSphere];
            minZ = -sizeXYZ[nowTypeSphere];
            maxZ = sizeXYZ[nowTypeSphere];

            if (x != 0 || z != 0)
            {
                if (mCameraScript.GetCurrentCameraIndex() == 0)
                {
                    Vector3 movement = new Vector3(x, 0, z) * moveSpeed * Time.deltaTime;
                    transform.Translate(movement, Space.World);
                    float clampedX = Mathf.Clamp(transform.position.x, minX, maxX);
                    float clampedZ = Mathf.Clamp(transform.position.z, minZ, maxZ);
                    transform.position = new Vector3(clampedX, transform.position.y, clampedZ);
                }
                else
                {
                    Vector3 forward = mainCamera.transform.forward;
                    Vector3 right = mainCamera.transform.right;
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

    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject != null && heldSphere != null && collision.gameObject != heldSphere.gameObject)
        {
            Debug.Log("TongsMoveScript OnCollisionEnter(),collision.gameObject != null && heldSphere != null && collision.gameObject != heldSphere.gameObject ");
            SpaceGameManager.Instance.GameOver();
        }
    }

    private void SettingSphereMove()
    {
        if (heldSphere == null)
        {
            SpaceSphereScript sphere = SpaceGameManager.Instance.GetObject(nextTypeSphere);
            HoldSphere(sphere);
        }
        else
        {
            
        }
    }

    public void FirstSettingSphereMove()//첫번째만 쓰이는 용도
    {
        if (heldSphere == null)
        {
            SpaceSphereScript sphere = SpaceGameManager.Instance.GetObject(nextTypeSphere);
            HoldSphere(sphere);
            // 다음 행성 이미지 설정
            nextPlanetImage.sprite = planetImageList[nextTypeSphere];
        }
        NextSphererInforMation();
    }

    public void DropSphere()
    {
       

        if (heldSphere != null && canDropSphere)
        {
            canDropSphere = false;
            StartCoroutine(ResetDropSphere());
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

            SphereCollider sphereCollider = heldSphere.GetComponent<SphereCollider>();
            if (sphereCollider != null)
            {
                sphereCollider.enabled = true;
            }

            heldSphere = null; // Clear the reference
        }
    }
    void AdjustPositionForNextSphereSize()
    {
        // Recalculate bounds based on the size of the next sphere.
        float newMinX = -sizeXYZ[nowTypeSphere];
        float newMaxX = sizeXYZ[nowTypeSphere];
        float newMinZ = -sizeXYZ[nowTypeSphere];
        float newMaxZ = sizeXYZ[nowTypeSphere];

        // Get current position
        Vector3 currentPosition = _transform.position;

        // Adjust position to stay within new bounds
        float adjustedX = Mathf.Clamp(currentPosition.x, newMinX, newMaxX);
        float adjustedZ = Mathf.Clamp(currentPosition.z, newMinZ, newMaxZ);

        // Apply adjusted position
        _transform.position = new Vector3(adjustedX, currentPosition.y, adjustedZ);
    }

    private IEnumerator ResetDropSphere()
    {
        yield return new WaitForSeconds(0.2f);
        canDropSphere = true;
    }
    public void HoldSphere(SpaceSphereScript sphere)
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
        if (SpaceGameManager.Instance.score >= 2000)//2천점 이상일 때 
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
            Vector3 hitPoint = hit.point;

            targetObject.transform.position = hitPoint;

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
        nowTypeSphere = nextTypeSphere;
        AdjustPositionForNextSphereSize();
        SettingSphereMove();
        nextTypeSphere = GetRandomNumber();
        // 다음 행성 이미지 설정
        nextPlanetImage.sprite = planetImageList[nextTypeSphere];
        
    }

    public void ChangeNextSphereType() 
    {
        if (changeCount == 0 && adPannel.activeSelf == false)
        {
            SpaceGameManager.Instance.ChangeAdUiPanel();
        }
        else
        {
            TriggerChangeCardOn();
        }
    }
    public void TriggerChangeCardOn()
    {
        int nowType = nextTypeSphere;

        while (nowType == nextTypeSphere)
        {
            nextTypeSphere = GetRandomNumber();
        }

        changeCount -= 1;

        textChangeCount.text = changeCount + "";
        nextPlanetImage.sprite = planetImageList[nextTypeSphere];
    }
    private IEnumerator ReleaseSphereWithDelay()
    {
        isReleasing = true;
        yield return new WaitForSeconds(0.1f); // Delay of 0.2 seconds

        // Existing logic for releasing the sphere
        float offsetX = (float)(random.NextDouble() * 0.02 - 0.01);
        float offsetZ = (float)(random.NextDouble() * 0.02 - 0.01);
        Vector3 newPosition = this.transform.position + new Vector3(offsetX, 0, offsetZ);
        heldSphere.transform.position = newPosition;

        heldSphere.SetTarget(null);
        Rigidbody sphereRb = heldSphere.GetComponent<Rigidbody>();
        if (sphereRb != null)
        {
            sphereRb.isKinematic = false;
        }
        SphereCollider sphereCollider = heldSphere.GetComponent<SphereCollider>();
        if (sphereCollider != null)
        {
            sphereCollider.enabled = true;
        }

        heldSphere = null;

        isReleasing = false; // Reset the flag
    }
}
