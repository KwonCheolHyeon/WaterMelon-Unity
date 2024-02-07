using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class IpoinerScript : MonoBehaviour
{
    private void Start()
    {
        // 부모 오브젝트를 시작으로 재귀적으로 모든 자식 오브젝트를 순회합니다.
        Traverse(transform);
    }

    private void Traverse(Transform obj)
    {
        // 현재 오브젝트의 자식 수만큼 반복합니다.
        foreach (Transform child in obj)
        {
            // 자식 오브젝트의 정보를 출력하거나 원하는 작업을 수행합니다.
            Debug.Log("자식 오브젝트 이름: " + child.name);

            if(child.CompareTag("Button"))
            {
                Button btn = child.GetComponent<Button>();
                btn.onClick.AddListener(() => SoundManagerScript.Instance.PlaySFXSound("UISFX"));
            }

            // 자식 오브젝트에 다시 Traverse 함수를 호출하여 자식의 자식을 확인합니다.
            Traverse(child);
        }
    }
}
