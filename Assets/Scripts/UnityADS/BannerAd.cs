using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Advertisements;
using UnityEngine.SceneManagement;
using static UnityEngine.Advertisements.Advertisement;

public class BannerAd : MonoBehaviour
{
    [SerializeField] private BannerPosition _bannerPosition = BannerPosition.BOTTOM_CENTER;
    [SerializeField] private string _androidAdUnitId = "Banner_Android";
    [SerializeField] private string _iOSAdUnitId = "Banner_iOS";
    private string _adUnitId = null;

    void Start()
    {
        // 현재 플랫폼에 맞는 Ad Unit ID 설정
#if UNITY_IOS
        _adUnitId = _iOSAdUnitId;
#elif UNITY_ANDROID
        _adUnitId = _androidAdUnitId;
#endif
    }

    // 배너 광고를 로드하고 표시하는 메서드
    public void OnAdButtonClicked()
    {
        Debug.Log("Attempting to load banner ad");

        // Set banner position
        Advertisement.Banner.SetPosition(_bannerPosition);

        // Load the banner ad
        BannerLoadOptions options = new BannerLoadOptions
        {
            loadCallback = OnBannerLoaded,
            errorCallback = OnBannerError
        };
        Advertisement.Banner.Load(_adUnitId, options);
    }

    private void OnBannerLoaded()
    {
        Debug.Log("Banner ad loaded successfully");
        // Display the banner ad
        Advertisement.Banner.Show(_adUnitId);
    }

    private void OnBannerError(string message)
    {
        Debug.LogError($"Banner ad failed to load: {message}");
    }
}