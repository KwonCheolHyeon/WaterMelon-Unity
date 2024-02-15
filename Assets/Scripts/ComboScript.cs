using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ComboScript : MonoBehaviour
{
    [SerializeField]
    GameObject plusImageObj;
    [SerializeField]
    private List<Sprite> comboSprites; // �޺� ���� ����

    public float scaleFactor = 1.1f; // ũ�⸦ ������ų ���
    public float scaleSpeed = 0.5f;   // ũ�� ���� �ӵ�
    public float delay = 0.5f;        // ũ�� ���� ������
    public float maxScale = 1.3f;     // �ִ� ũ��

    public float smoothTime = 0.5f;   // ������ ���� �ð�
    private float comboTime = 3.0f;

    private Vector3 initialScale = Vector3.one;      // �ʱ� ������
    private Vector3 targetScale;       // ��ǥ ������
    private Vector3 velocity = Vector3.one; // ������ ���� �ʱ� �ӵ�

    private bool scalingUp = false;      // ���� �� ����
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

        // ���� �� ����
        while (true)
        {
            if (!scalingUp)
            {
                scalingUp = true;

                // ����
                while (Vector3.Distance(transform.localScale, targetScale) > 0.01f)
                {
                    // �ε巴�� �����ϱ� ���� ����
                    transform.localScale = Vector3.SmoothDamp(transform.localScale, targetScale, ref velocity, smoothTime);
                    yield return null;
                }

                // ����
                while (Vector3.Distance(transform.localScale, initialScale) > 0.01f)
                {
                    // �ε巴�� �����ϱ� ���� ����
                    transform.localScale = Vector3.SmoothDamp(transform.localScale, initialScale, ref velocity, smoothTime);
                    yield return null;
                }

                // ũ�⸦ �ʱ� �����Ϸ� �����Ͽ� ������ �����մϴ�.
                transform.localScale = initialScale;
                scalingUp = false;
            }
            yield return null;
        }
    }
}
