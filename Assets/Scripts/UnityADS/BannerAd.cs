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
    private RectTransform uiElement; // 조정하고자 하는 UI 요소
    float bannerHeight = 140.0f; // 배너 높이를 가정한 값

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
            // 다른 씬에서는 배너 광고 숨기기
            HideBannerAd();
        }
    }
   
    void InitializeAds()
    {
        // 현재 플랫폼에 맞는 Ad Unit ID 설정
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
            // UI 요소의 현재 위치를 가져옵니다.
            Vector3 currentPosition = uiElement.anchoredPosition;

            // UI 요소를 배너 높이만큼 위로 이동시킵니다.
            uiElement.anchoredPosition = new Vector3(currentPosition.x, currentPosition.y + bannerHeight, currentPosition.z);
        }
    }

    void OnBannerLoaded()
    {
        Debug.Log("Banner Ad successfully loaded.");
        Advertisement.Banner.Show(_adUnitId); // 배너 광고 표시
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
