using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Advertisements;
using UnityEngine.SceneManagement;

public class BannerAd : MonoBehaviour
{
    [SerializeField] BannerPosition _bannerPosition = BannerPosition.BOTTOM_CENTER;
    [SerializeField] string _androidAdUnitId = "Banner_Android";
    [SerializeField] string _iOSAdUnitId = "Banner_iOS";
    private string _adUnitId = null;
    private RectTransform uiElement; // �����ϰ��� �ϴ� UI ���
    float bannerHeight = 140.0f; // ��� ���̸� ������ ��

    void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "SlimeScene" || scene.name == "SpaceGameScene")
        {
            InitializeAds();
            FindAndAdjustUI();
        }
        else
        {
            // �ٸ� �������� ��� ���� �����
            HideBannerAd();
        }
    }
   
    void InitializeAds()
    {
        // ���� �÷����� �´� Ad Unit ID ����
#if UNITY_IOS
        _adUnitId = _iOSAdUnitId;
#elif UNITY_ANDROID
        _adUnitId = _androidAdUnitId;
#endif


        // Set banner position and load banner
        Advertisement.Banner.SetPosition(_bannerPosition);
        LoadBanner();
    }

    public void LoadBanner()
    {
        Debug.Log("Loading Banner Ad");
        BannerLoadOptions options = new BannerLoadOptions
        {
            loadCallback = OnBannerLoaded,
            errorCallback = OnBannerError
        };
        Advertisement.Banner.Load(_adUnitId, options);
    }

    void AdjustUIForBanner()
    {
        if (uiElement != null)
        {
            // UI ����� ���� ��ġ�� �����ɴϴ�.
            Vector3 currentPosition = uiElement.anchoredPosition;

            // UI ��Ҹ� ��� ���̸�ŭ ���� �̵���ŵ�ϴ�.
            uiElement.anchoredPosition = new Vector3(currentPosition.x, currentPosition.y + bannerHeight, currentPosition.z);
        }
    }

    void OnBannerLoaded()
    {
        Debug.Log("Banner Ad successfully loaded.");
        Advertisement.Banner.Show(_adUnitId); // ��� ���� ǥ��
        AdjustUIForBanner();
    }

    void OnBannerError(string message)
    {
        Debug.LogError($"Banner Ad failed to load: {message}");
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    void FindAndAdjustUI()
    {
        GameObject moveUIObject = GameObject.Find("Canvas/MoveUI");
        if (moveUIObject != null)
        {
            uiElement = moveUIObject.GetComponent<RectTransform>();
        }
    }
    void HideBannerAd()
    {
        Advertisement.Banner.Hide();
    }

}
