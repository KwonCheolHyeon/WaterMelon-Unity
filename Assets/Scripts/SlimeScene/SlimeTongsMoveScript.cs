using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SlimeTongsMoveScript : MonoBehaviour
{
    private Camera mainCamera;
    private CameraMoveScript mCameraScript;

    private bool _isMoving = true;
    private float moveSpeed = 4.0f;
    private float[] sizeXYZ = new float[5];
    private float minX, maxX, minZ, maxZ;

    private SlimePrefabScript heldSlime = null;

    private Transform _transform;

    //연속 떨구는거 딜레이 주기 위한 bool값
    private bool isReleasing = false;

    //랜덤 시스템관련
    private System.Random random = new System.Random();

    //라인 보여주는 함수
    private LineRenderer lineRenderer;
    public float rayLength = 10f;
    [SerializeField]
    private GameObject targetObject;// 오브젝트와 닿을 때 생기는 구체
    //조이스틱 관련
    public VariableJoystick variableJoystick;
    private bool isTongsMoving = false;
    public bool IsTongsMoving() { return isTongsMoving; }
    //단계별 슬라임 이미지
    [SerializeField]
    private List<Sprite> slimeImageList = new List<Sprite>();

    //다음 나올 과일을 보여주는 관련
    [SerializeField]
    private Image nextSlimeImage;
    private int nextTypeSlime;
    private int nowTypeSlime;
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

    public float dotSpacing = 0.2f; // 점과 점 사이의 간격
    public float lineWidth = 0.1f; // 선의 너비

    void Start()
    {
        changeCount = 2;
        nextTypeSlime = 0;
        nowTypeSlime = nextTypeSlime;
        mainCamera = Camera.main;
        mCameraScript = mainCamera.GetComponent<CameraMoveScript>();
        sizeXYZ[0] = 1.77f;
        sizeXYZ[1] = 1.67f;
        sizeXYZ[2] = 1.61f;
        sizeXYZ[3] = 1.48f;
        sizeXYZ[4] = 1.35f;
        minX = -sizeXYZ[nowTypeSlime];
        maxX = sizeXYZ[nowTypeSlime];
        minZ = -sizeXYZ[nowTypeSlime];
        maxZ = sizeXYZ[nowTypeSlime];
        _transform = transform;
        lineRenderer = GetComponent<LineRenderer>();
        FirstSettingSphereMove();


    }

    private void Update()
    {
        if (lineRenderer != null)
        {
            lineRenderer.enabled = (heldSlime != null);
            targetObject.SetActive((heldSlime != null));
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

            minX = -sizeXYZ[nowTypeSlime];
            maxX = sizeXYZ[nowTypeSlime];
            minZ = -sizeXYZ[nowTypeSlime];
            maxZ = sizeXYZ[nowTypeSlime];

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
            //SlimeGameManager.Instance.GameOver();
        }
    }

    public void FirstSettingSphereMove()//첫번째만 쓰이는 용도
    {
        if (heldSlime == null)
        {
            SlimePrefabScript sphere = SlimeGameManager.Instance.GetObject(0);
            HoldSlime(sphere);

            //처음 시작시 다음에 나올 슬라임 nextSlimeImage에 표시
            nextTypeSlime = GetRandomNumber();
            nextSlimeImage.sprite = slimeImageList[nextTypeSlime];
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
        if (SlimeGameManager.Instance.score >= 2000)//2천점 이상일 때 
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
            hitPoint.y += 0.3f;
            targetObject.transform.position = hitPoint;
            targetObject.transform.rotation = heldSlime.transform.rotation;

            //SetLineRendererGradientAtPoint(hit.distance / rayLength);
            lineRenderer.SetPosition(1, hit.point);
        }
        else
        {
            //ResetLineRendererGradient();
            lineRenderer.SetPosition(1, start + direction * rayLength);
        }
    }

    public void NextSphererInforMation() // 다음 구체에 대한 정보
    {
        nowTypeSlime = nextTypeSlime;
        SettingSphereMove();
        AdjustPositionForNextSphereSize();

        nextTypeSlime = GetRandomNumber();
        nextSlimeImage.sprite = slimeImageList[nextTypeSlime];
    
    }
    public void ChangeNextSphereType()
    {
        if (changeCount == 0 && adPannel.activeSelf == false)
        {
            SlimeGameManager.Instance.ChangeAdUiPanel();
        }
        else
        {
            TriggerChangeCardOn();
        }

    
    }

    void AdjustPositionForNextSphereSize()
    {
        // Recalculate bounds based on the size of the next sphere.
        float newMinX = -sizeXYZ[nowTypeSlime];
        float newMaxX = sizeXYZ[nowTypeSlime];
        float newMinZ = -sizeXYZ[nowTypeSlime];
        float newMaxZ = sizeXYZ[nowTypeSlime];

        // Get current position
        Vector3 currentPosition = _transform.position;

        // Adjust position to stay within new bounds
        float adjustedX = Mathf.Clamp(currentPosition.x, newMinX, newMaxX);
        float adjustedZ = Mathf.Clamp(currentPosition.z, newMinZ, newMaxZ);

        // Apply adjusted position
        _transform.position = new Vector3(adjustedX, currentPosition.y, adjustedZ);

    }

    public void TriggerChangeCardOn() 
    {
        int nowType = nextTypeSlime;

        while (nowType == nextTypeSlime)
        {
            nextTypeSlime = GetRandomNumber();
        }

        changeCount -= 1;

        textChangeCount.text = changeCount + "";
        nextSlimeImage.sprite = slimeImageList[nextTypeSlime];
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
