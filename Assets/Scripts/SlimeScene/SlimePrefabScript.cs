using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class SlimePrefabScript : MonoBehaviour
{
    private List<string> tagsToCheck = new List<string> { "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "ten" };

    private Vector3 nextScale = new Vector3(0.4f, 0.4f, 0.4f);
    private int type = 0;

    private Transform targetToFollow;
    private Rigidbody rigid;
    private MeshCollider meshColl;

    // �ܰ躰 ��Ƽ������ �����ϴ� List ����
    [SerializeField]
    private List<Material> materials;
    [SerializeField]
    private GameObject collEffect;

    private Vector3 resetPos = new Vector3(0, 30, 0);

    private bool gameOverRun = false;
    private bool _isMerge = false;
    private bool isBottom = false;

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
        meshColl = GetComponent<MeshCollider>();
    }

    private void Start()
    {
        // ������ ȸ�� ���� �����մϴ�.
        Vector3 randomRotation = new Vector3(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360));

        // ���� ������Ʈ�� ȸ�� ���� �����մϴ�.
        transform.rotation = Quaternion.Euler(randomRotation);

        rigid.mass = 5f;
    }

    private void OnDisable()
    {
        isBottom = false;
        targetToFollow = null;

        // �浹 �� ������� ������ ��ġ�� �ʱ�ȭ
        this.transform.position = resetPos;
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
        if (SlimeGameManager.Instance.GameOverState)
        {
            return;
        }

        if (isBottom == false && (collision.gameObject.CompareTag("Case") || tagsToCheck.Contains(collision.gameObject.tag)))
        {
            isBottom = true;
            SlimeGameManager.Instance.SphereBottomTrue();
        }
        if (targetToFollow != null)//���Ը� ����ٴϴ� ����
        {
            if (tagsToCheck.Contains(collision.gameObject.tag))
            {
                SlimeGameManager.Instance.GameOver();
                GameOverState();
            }
        }
        if (tagsToCheck.Contains(collision.gameObject.tag))
        {
            SlimePrefabScript otherSphere = collision.gameObject.GetComponent<SlimePrefabScript>();
            if (otherSphere != null && otherSphere.tag == this.tag && !isMerge && !otherSphere.isMerge && this.tag != "ten")
            {
                // �������� ���� ����
                VibrationManager.Instance.CreateOneShot(20);

                float meX = transform.position.x;
                float meY = transform.position.y;
                float meZ = transform.position.z;
                float otherX = otherSphere.transform.position.x;
                float otherY = otherSphere.transform.position.y;
                float otherZ = otherSphere.transform.position.z;

                if (meY < otherY || (meY == otherY && meX > otherX) || (meY == otherY && meZ > otherZ)) //y���� �ٸ��� , y���� ������ x���� �ٸ���,y���� ���� x�൵ ������ z���� �ٸ���
                {
                    otherSphere.HideSphereObject(transform.position);
                    SettingChangeSphere();
                }
            }
        }

       

        if ((collision.gameObject.CompareTag("GameOverTag")))
        {
            SlimeGameManager.Instance.GameOver();
            GameOverState();
            Debug.Log("GAMEover");
        }

        rigid.mass = 1.0f;
    }

    private void OnCollisionStay(Collision collision)
    {
        if (SlimeGameManager.Instance.GameOverState)
        {
            return;
        }

        if (tagsToCheck.Contains(collision.gameObject.tag))
        {
            SlimePrefabScript otherSphere = collision.gameObject.GetComponent<SlimePrefabScript>();
            if (otherSphere != null && otherSphere.tag == this.tag && !isMerge && !otherSphere.isMerge && this.tag != "ten")
            {
                float meX = transform.position.x;
                float meY = transform.position.y;
                float meZ = transform.position.z;
                float otherX = otherSphere.transform.position.x;
                float otherY = otherSphere.transform.position.y;
                float otherZ = otherSphere.transform.position.z;

                if (meY < otherY || (meY == otherY && meX > otherX) || (meY == otherY && meZ > otherZ)) //y���� �ٸ��� , y���� ������ x���� �ٸ���,y���� ���� x�൵ ������ z���� �ٸ���
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

        // type ���� �´� ��Ƽ���� ����
        GetComponent<MeshRenderer>().material = materials[type];

        switch (type) //Ÿ�� ���� ���� ���ش�.(��Ų ����)
        {
            case 0:
                break;
            case 1:
                break;
            case 2:
                break;
            case 3:
                break;
            case 4:
                break;
            case 5:
                break;
            case 6:
                break;
            case 7:
                break;
            case 8:
                break;
            case 9:
                break;
            case 10:
                break;
            default:
                Debug.LogError("SettingSphere type ����");
                break;
        }

    }

    public void SettingChangeSphere()
    {
        isMerge = true;

        SlimeGameManager.Instance.SetGameScore(type);

        rigid.velocity = Vector3.zero;
        rigid.angularVelocity = Vector3.zero;

        type += 1;
        this.tag = tagsToCheck[type];

        transform.localScale += nextScale;

        // type ���� �´� ��Ƽ���� ����
        GetComponent<MeshRenderer>().material = materials[type];

        switch (type) //Ÿ�� ���� ���� ���ش�.(��Ų ����)
        {
            case 0:
                break;
            case 1:
                break;
            case 2:
                break;
            case 3:
                break;
            case 4:
                break;
            case 5:
                break;
            case 6:
                break;
            case 7:
                break;
            case 8:
                break;
            case 9:
                break;
            case 10:
                break;
            default:
                Debug.LogError("SettingSphere type ����");
                break;
        }
        isMerge = false;
    }

    public void HideSphereObject(Vector3 targetPos)
    {
        isMerge = true;
        rigid.useGravity = false;
        meshColl.enabled = false;

        // �浹 ���� ���
        SoundManagerScript.Instance.PlaySFXSound("SlimeSFX");

        // �浹 ����Ʈ ����
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
        SlimeGameManager.Instance.ReturnObject(this);
    }

    private void GameOverState()
    {
        // Stop all collision by disabling the Rigidbody and Collider
        rigid.isKinematic = true;
        meshColl.enabled = false;
    }

    private void CreateCollisionEffect()
    {
        GameObject effect = GameObject.Instantiate(collEffect);

        // ��ƼŬ�� ���ۻ����� SlimePrefab�� ����� ������ ����
        var ma = effect.GetComponent<ParticleSystem>().main;
        Color color = materials[type].GetColor("_Fresnel_Color");
        color.a = 1f;
        ma.startColor = color;

        effect.transform.position = this.transform.position;
        effect.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
    }
}
