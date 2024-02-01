using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ProgressScript : MonoBehaviour
{
    [SerializeField]
    private Slider sliderProgress;
    [SerializeField]
    private TextMeshProUGUI textProgressData;
    [SerializeField]
    private float progressTime; // �ε��� ��� �ð�

    public void Play(UnityAction _action=null)
    {
        StartCoroutine(OnProgress(_action));
    }

    private IEnumerator OnProgress(UnityAction _action)
    {
        float current = 0;
        float percent = 0;

        while(percent <1)
        {
            current += Time.deltaTime;
            percent = current / progressTime;

            // Text ���� ����
            textProgressData.text = $"Now Loading... {sliderProgress.value * 100:F0}%";
            // Slider �� ����
            sliderProgress.value = Mathf.Lerp(0, 1, percent);

            yield return null;
        }

        // _action�� null�� �ƴϸ� _action �޼ҵ� ����
        _action?.Invoke();
    }
}
