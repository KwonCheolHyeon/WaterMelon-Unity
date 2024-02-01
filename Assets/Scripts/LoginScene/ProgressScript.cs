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
    private float progressTime; // 로딩바 재생 시간

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

            // Text 정보 설정
            textProgressData.text = $"Now Loading... {sliderProgress.value * 100:F0}%";
            // Slider 값 설정
            sliderProgress.value = Mathf.Lerp(0, 1, percent);

            yield return null;
        }

        // _action이 null이 아니면 _action 메소드 실행
        _action?.Invoke();
    }
}
