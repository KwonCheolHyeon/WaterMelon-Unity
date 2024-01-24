using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class SpherePrefabScript : MonoBehaviour
{
    private List<string> tagsToCheck = new List<string> { "one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "ten", "eleven" };
    private int type = 0;
    private bool isBottom = false;
    private Transform targetToFollow;
    private bool _isMerge = false;

    private Vector3 nextScale = new Vector3(0.2f,0.2f,0.2f);
    private Rigidbody rigid;
    private SphereCollider sphereCol;
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
    }

    void Start()
    {
      
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
        if (targetToFollow != null)
        {
            // Follow the target's position
            transform.position = targetToFollow.position;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (isBottom == false && (collision.gameObject.CompareTag("Case") || tagsToCheck.Contains(collision.gameObject.tag)))
        {
            isBottom = true;
            GameManager.Instance.SphereBottomTrue();
        }

        if (tagsToCheck.Contains(collision.gameObject.tag)) 
        {
            SpherePrefabScript otherSphere = collision.gameObject.GetComponent<SpherePrefabScript>();

            if (otherSphere != null && otherSphere.tag == this.tag && !isMerge && !otherSphere.isMerge && this.tag != "eleven")
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


    void OnTriggerEnter(Collider other)
    {
       
    }

    public void SettingSphere(int _index,float _size) 
    {
        type = _index;
        this.tag = tagsToCheck[_index];
        transform.localScale = new Vector3(_size, _size, _size);

        switch (type) //타입 별로 세팅 해준다.(스킨 예상)
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
            case 11:
                break;
            default:
                Debug.LogError("SettingSphere type 오류");
                break;
        }

    }

    public void SettingChangeSphere() 
    {
        isMerge = true;

        rigid.velocity = Vector3.zero;
        rigid.angularVelocity = Vector3.zero;

        type += 1;
        this.tag = tagsToCheck[type];

        transform.localScale += nextScale;

        switch (type) //타입 별로 세팅 해준다.(스킨 예상)
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
            case 11:
                break;
            default:
                Debug.LogError("SettingSphere type 오류");
                break;
        }
        isMerge = false;
    }

    public void HideSphereObject(Vector3 targetPos) 
    {
        isMerge = true;
        rigid.useGravity = false;
        sphereCol.enabled = false;

        StartCoroutine(HideRoutine(targetPos));
    }

    IEnumerator HideRoutine(Vector3 targetPos) 
    {
        int frameCount = 0;
        while (frameCount< 30) //흡수 하는 느낌을 주기 위해
        {
            frameCount++;
            transform.position = Vector3.Lerp(transform.position, targetPos, 0.2f);
            yield return null;
        }
        
        isMerge = false;
        GameManager.Instance.ReturnObject(this);
    }




}
