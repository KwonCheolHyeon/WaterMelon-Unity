using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ComboScript : MonoBehaviour
{
    [SerializeField]
    GameObject plusImageObj;
    [SerializeField]
    private List<Sprite> comboSprites; // 콤보 숫자 모음

    public float scaleFactor = 1.1f; // 크기를 증가시킬 배수
    public float scaleSpeed = 0.5f;   // 크기 증가 속도
    public float delay = 0.5f;        // 크기 증가 딜레이
    public float maxScale = 1.3f;     // 최대 크기

    public float smoothTime = 0.5f;   // 보간에 사용될 시간
    private float comboTime = 3.0f;

    private Vector3 initialScale = Vector3.one;      // 초기 스케일
    private Vector3 targetScale;       // 목표 스케일
    private Vector3 velocity = Vector3.one; // 보간에 사용될 초기 속도

    private bool scalingUp = false;      // 증가 중 여부
    private bool isBlinking = false;

    private float blinkSpeed = 2.0f;

    private Image image;

    private void OnEnable()
    {
        image = GetComponent<Image>();
    }

    private void OnDisable()
    {
        plusImageObj.SetActive(false);
    }

    private void Update()
    {
        comboTime -= Time.deltaTime;
        if(comboTime < 1.0f)
        {
            float alpha = Mathf.PingPong(Time.time * blinkSpeed, 1.0f);
            image.color = new Color(image.color.r, image.color.g, image.color.b, alpha);
        }
    }

    public void SetComboImage(int _index)
    {
        if ((_index >= 9))
        {
            _index = 9;
            if(plusImageObj.activeSelf == false)
                plusImageObj.SetActive(true);
        }

        if(image == null)
            image = GetComponent<Image>();

        image.sprite = comboSprites[_index];

        targetScale = initialScale * maxScale;

        StartCoroutine(ScaleOverTime());
    }

    private IEnumerator ScaleOverTime()
    {
        this.transform.localScale = initialScale;
        scalingUp = false;
        smoothTime = 0.5f;
        image.color = Color.white;
        comboTime = 3.0f;

        // 증가 및 감소
        while (true)
        {
            if (!scalingUp)
            {
                scalingUp = true;

                // 증가
                while (Vector3.Distance(transform.localScale, targetScale) > 0.01f)
                {
                    // 부드럽게 증가하기 위해 보간
                    transform.localScale = Vector3.SmoothDamp(transform.localScale, targetScale, ref velocity, smoothTime);
                    yield return null;
                }

                // 감소
                while (Vector3.Distance(transform.localScale, initialScale) > 0.01f)
                {
                    // 부드럽게 감소하기 위해 보간
                    transform.localScale = Vector3.SmoothDamp(transform.localScale, initialScale, ref velocity, smoothTime);
                    yield return null;
                }

                // 크기를 초기 스케일로 설정하여 오류를 방지합니다.
                transform.localScale = initialScale;
                scalingUp = false;
            }
            yield return null;
        }
    }
}
