using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceSphereScript : MonoBehaviour
{
    private List<string> tagsToCheck = new List<string> { "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "ten" };
    private int type = 0;
    private bool isBottom = false;
    private Transform targetToFollow;
    private bool _isMerge = false;

    private Vector3 nextScale = new Vector3(0.0225f, 0.0225f, 0.0225f);
    private Rigidbody rigid;
    private SphereCollider sphereCol;
    private float originalRadius;
    private MeshFilter thisMesh;
    private bool gameOverRun = false;

    [SerializeField]
    private Mesh[] meshes;

    private GameObject collEffect;
    public void SetCollEffect(GameObject _effect) { collEffect = _effect; }



    public void SpaceMeshSetting() 
    {
        Mesh[] masterMesh = SpaceGameManager.Instance.GetSapceMesh();
        meshes = masterMesh;
    }

    public bool isMerge
    {
        get
        {
            return _isMerge;
        }
        set
        {
            // Perform any additional logic when isMerge changes
            if (_isMerge != value)
            {
                _isMerge = value;
            }
        }
    }

    public void SetTarget(Transform target)
    {
        targetToFollow = target;
    }

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        sphereCol = GetComponent<SphereCollider>();
        thisMesh = GetComponent<MeshFilter>();
    }


    void Start()
    {
        originalRadius = sphereCol.radius;
    }

    private void OnDisable()
    {
        isBottom = false;
        targetToFollow = null;
    }
    private void OnEnable()
    {

    }

    void Update()
    {

    }
    void FixedUpdate()
    {
        if (targetToFollow != null)
        {
            // Follow the target's position
            transform.position = targetToFollow.position;
        }
    }
    void OnCollisionEnter(Collision collision)
    {
        if (SpaceGameManager.Instance.GameOverState)
        {
            return;
        }

        if (isBottom == false && (collision.gameObject.CompareTag("Case") || tagsToCheck.Contains(collision.gameObject.tag)))
        {
            isBottom = true;
            SpaceGameManager.Instance.SphereBottomTrue();
        }

        if (targetToFollow != null)//집게를 따라 다니는 상태
        {
            if (tagsToCheck.Contains(collision.gameObject.tag))
            {
                SpaceGameManager.Instance.GameOver();
                GameOverState();
            }
        }

        if (tagsToCheck.Contains(collision.gameObject.tag))
        {
            SpaceSphereScript otherSphere = collision.gameObject.GetComponent<SpaceSphereScript>();

            if (otherSphere != null && otherSphere.tag == this.tag && !isMerge && !otherSphere.isMerge && this.tag != "ten")
            {
                float meX = transform.position.x;
                float meY = transform.position.y;
                float meZ = transform.position.z;
                float otherX = otherSphere.transform.position.x;
                float otherY = otherSphere.transform.position.y;
                float otherZ = otherSphere.transform.position.z;

                if (meY < otherY || (meY == otherY && meX > otherX) || (meY == otherY && meZ > otherZ)) //y축이 다를때 , y축이 같으면 x축이 다를때,y축이 같고 x축도 같으면 z축이 다를때
                {
                    otherSphere.HideSphereObject(transform.position);
                    SettingChangeSphere();
                }
            }
        }

       

        if ((collision.gameObject.CompareTag("GameOverTag")))
        {
            Debug.Log("GameOver SpaceSphereScript OnCollisionEnter() collision.gameObject.CompareTag");
            SpaceGameManager.Instance.GameOver();
            GameOverState();

        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (SpaceGameManager.Instance.GameOverState)
        {
            return;
        }

        if (tagsToCheck.Contains(collision.gameObject.tag))
        {
            SpaceSphereScript otherSphere = collision.gameObject.GetComponent<SpaceSphereScript>();

            if (otherSphere != null && otherSphere.tag == this.tag && !isMerge && !otherSphere.isMerge && this.tag != "ten")
            {
                float meX = transform.position.x;
                float meY = transform.position.y;
                float meZ = transform.position.z;
                float otherX = otherSphere.transform.position.x;
                float otherY = otherSphere.transform.position.y;
                float otherZ = otherSphere.transform.position.z;

                if (meY < otherY || (meY == otherY && meX > otherX) || (meY == otherY && meZ > otherZ)) //y축이 다를때 , y축이 같으면 x축이 다를때,y축이 같고 x축도 같으면 z축이 다를때
                {
                    otherSphere.HideSphereObject(transform.position);
                    SettingChangeSphere();
                }
            }
        }
    }

    public void SettingSphere(int _index, float _size)
    {
        type = _index;
        this.tag = tagsToCheck[_index];
        transform.localScale = new Vector3(_size, _size, _size);

        if (meshes != null && _index >= 0 && _index < meshes.Length)
        {
            thisMesh.mesh = meshes[_index];
        }
        else
        {
            Debug.LogError("스페이스 구 스크립트 SettingSphere 오류");

            if (meshes == null) 
            {
                Debug.LogError("meshes null 에러");
            }
            if (_index < 0) 
            {
                Debug.LogError("_index 에러");
            }
            if (_index >= meshes.Length)
            {
                Debug.LogError("meshes.Length");
            }
        }
    }

    public void SettingChangeSphere()
    {
        isMerge = true;

        SpaceGameManager.Instance.SetGameScore(type);

        rigid.velocity = Vector3.zero;
        rigid.angularVelocity = Vector3.zero;

        type += 1; // Increment the type to the next level
        this.tag = tagsToCheck[type]; // Update the tag

        transform.localScale += nextScale; // Adjust the scale as needed

        // Update the mesh based on the new type
        if (meshes != null && type >= 0 && type < meshes.Length)
        {
            thisMesh.mesh = meshes[type];

            // Check if the type is 7 to adjust the SphereCollider's radius
            if (type == 7)
            {
                sphereCol.radius = 4.86f;
            }
            else
            {
                sphereCol.radius = originalRadius;
            }
        }
        else
        {
            Debug.LogError("스페이스 프리팹 스크립 SettingChangeSphere()");
        }

        isMerge = false; // Resetting merge state
    }

    public void HideSphereObject(Vector3 targetPos)
    {
        isMerge = true;
        rigid.useGravity = false;
        sphereCol.enabled = false;

        // 충돌 사운드 생성
        SoundManagerScript.Instance.PlaySFXSound("PlanetSFX", 0.5f);

        // 충돌 이펙트 생성
        CreateCollisionEffect();

        StartCoroutine(HideRoutine(targetPos));
    }
    IEnumerator HideRoutine(Vector3 targetPos)
    {
        float duration = 0.1f; // Adjust this value to control the duration of the lerping
        float elapsedTime = 0.0f;

        Vector3 startPosition = transform.position;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration); // Ensure t stays within [0, 1]

            transform.position = Vector3.Lerp(startPosition, targetPos, t);
            yield return null;
        }

        isMerge = false;
        SpaceGameManager.Instance.ReturnObject(this,type);
    }

    private void GameOverState()
    {
        // Stop all collision by disabling the Rigidbody and Collider
        rigid.isKinematic = true;
        sphereCol.enabled = false;
    }

    private void CreateCollisionEffect()
    {
        GameObject effect = GameObject.Instantiate(collEffect);
        effect.transform.position = this.transform.position;
        effect.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
    }
}
