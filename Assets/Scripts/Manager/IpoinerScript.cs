using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class IpoinerScript : MonoBehaviour
{
    private void Start()
    {
        // �θ� ������Ʈ�� �������� ��������� ��� �ڽ� ������Ʈ�� ��ȸ�մϴ�.
        Traverse(transform);
    }

    private void Traverse(Transform obj)
    {
        // ���� ������Ʈ�� �ڽ� ����ŭ �ݺ��մϴ�.
        foreach (Transform child in obj)
        {
            // �ڽ� ������Ʈ�� ������ ����ϰų� ���ϴ� �۾��� �����մϴ�.
            Debug.Log("�ڽ� ������Ʈ �̸�: " + child.name);

            if(child.CompareTag("Button"))
            {
                Button btn = child.GetComponent<Button>();
                btn.onClick.AddListener(() => SoundManagerScript.Instance.PlaySFXSound("UISFX"));
            }

            // �ڽ� ������Ʈ�� �ٽ� Traverse �Լ��� ȣ���Ͽ� �ڽ��� �ڽ��� Ȯ���մϴ�.
            Traverse(child);
        }
    }
}
