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
            GameManager.Instance.SphereTriggerTrue();
        }
    }


    void OnTriggerEnter(Collider other)
    {
       
    }

    public void SettingSphere(int _index,float _size) 
    {
        
    }

    public void SettingChangeSphere() 
    {
    
    }
}
