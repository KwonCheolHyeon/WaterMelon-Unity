using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpherePrefabScript : MonoBehaviour
{
    private List<string> tagsToCheck = new List<string> { "one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "ten", "eleven" };
    private int type = 0;
    private bool isBottom = false;
    private Transform targetToFollow;
    public bool isMerge;

    public int GetSphereType() { return type; }
    public void SetTarget(Transform target)
    {
        targetToFollow = target;
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

        //SpherePrefabScript otherSphere = collision.gameObject.GetComponent<SpherePrefabScript>();

        //if (otherSphere != null &&  !isMerge &&otherSphere.tag == this.tag )
        //{
        //    Vector3 spawnPosition = (this.transform.position + otherSphere.transform.position) / 2;
           

            
        //    Destroy(this.gameObject);
        //    Destroy(otherSphere.gameObject);
        //}

    }


    void OnTriggerEnter(Collider other)
    {
       
    }

    public void SettingSphere(int _index,float _size) 
    {
        type = _index;
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
    
    }
}
