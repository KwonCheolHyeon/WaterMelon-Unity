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
    private float bannerHeight = 140.0f; // 배너 높이를 가정한 값

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
        float currentAspectRatio = Screen.width / (float)Screen.height;

        // Define aspect ratio thresholds
        float aspectRatio18_9 = 18f / 9f;
        float aspectRatio16_9 = 16f / 9f;

        // Adjust estimatedBannerHeight based on aspect ratio
        float estimatedBannerHeight;
        if (currentAspectRatio >= aspectRatio18_9)
        {
            // For 18:9 screens, the banner might need to be relatively smaller
            estimatedBannerHeight = Screen.height * 0.07f; // Adjust this value as needed
        }
        else if (currentAspectRatio >= aspectRatio16_9)
        {
            // For 16:9 screens, adjust the height appropriately
            estimatedBannerHeight = Screen.height * 0.08f; // Adjust this value as needed
        }
        else
        {
            // Default or other aspect ratios
            estimatedBannerHeight = Screen.height * 0.0f; // Adjust this value as needed
        }

        if (uiElement != null)
        {
            // Adjust the UI element's position based on the calculated banner height
            Vector3 currentPosition = uiElement.anchoredPosition;
            uiElement.anchoredPosition = new Vector3(currentPosition.x, currentPosition.y + estimatedBannerHeight, currentPosition.z);
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
