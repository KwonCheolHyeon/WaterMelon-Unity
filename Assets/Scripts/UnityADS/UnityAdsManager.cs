using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.SceneManagement;

public class UnityAdsManager : MonoBehaviour
{

    private static UnityAdsManager instance = null;
    [SerializeField] private AdsInitializer adsInitializer;
    [SerializeField] private InterstitialAd interstitialAd;
    [SerializeField] private RewardedAds rewardedAds;
    [SerializeField] private BannerAd bannerAd;
    private GameObject gameManager;
    private int sceneType = 0;//0 = 슬라임 , 1 = 우주
    private int rewardType = 0;

    public void RewardType(int _type)
    {
        rewardType = _type;
    }

    public static UnityAdsManager Instance
    {
        get
        {
            if (null == instance)
            {
                return null;
            }
            return instance;
        }
    }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // 이 오브젝트를 씬 로딩 시 파괴하지 않습니다.

            // 씬이 로드될 때 호출될 함수를 이벤트에 등록합니다.
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else if (instance != this)
        {
            Destroy(gameObject); // 중복 인스턴스를 방지합니다.
        }
    }

    // 씬이 로드될 때마다 호출될 함수입니다.
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("Loaded new scene: " + scene.name);

        if (scene.name == "SlimeScene" || scene.name == "SpaceGameScene")
        {
            // GameManager를 찾습니다.
            gameManager = GameObject.Find("GameManager");

            // sceneType을 설정합니다. (0: 슬라임 씬, 1: 우주 씬)
            sceneType = scene.name == "SlimeScene" ? 0 : 1;

            // 배너 광고를 로드 및 표시합니다.
            LoadAndShowBannerAd();
            Advertisement.Banner.Hide();
        }
    }

    void OnDestroy()
    {
        // 오브젝트가 파괴될 때 이벤트에서 함수를 제거하여 메모리 누수를 방지합니다.
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    private void LoadAndShowBannerAd()
    {
        if (bannerAd != null)
        {

        }
        else
        {
            Debug.LogError("BannerAd 컴포넌트가 할당되지 않았습니다.");
        }
    }

    public void Request()
    {
        rewardedAds.LoadAd();
    }

    public void RewardOn()
    {
        if (sceneType == 0)//슬라임
        {
            if (rewardType == 0)
            {
                SlimeGameManager.Instance.RewardShackStageCard();
            }
            else if (rewardType == 1)
            {
                SlimeGameManager.Instance.RewardChangeCard();
            }
            else
            {
                Debug.LogError("보상 오류 RewardOn");
            }
        }
        else if (sceneType == 1)
        {
            if (rewardType == 0)
            {
                SpaceGameManager.Instance.RewardShackStageCard();
            }
            else if (rewardType == 1)
            {
                SpaceGameManager.Instance.RewardChangeCard();
            }
            else
            {
                Debug.LogError("보상 오류 RewardOn");
            }
        }
    }

    public void GameOver()
    {
        interstitialAd.LoadAd();

        StartCoroutine(ShowInterstitialAdWithDelay(1.0f));
    }
    private IEnumerator ShowInterstitialAdWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay); // Wait for the specified delay

        interstitialAd.ShowAdOnGameOver();
    }
    public void GameScoreSave()
    {
        if (sceneType == 0)//슬라임
        {
            Debug.Log("슬라임 씬 게임 오버");
            SlimeGameManager.Instance.SaveGameOverScore();
        }
        else if (sceneType == 1)
        {
            SpaceGameManager.Instance.SaveGameOverScore();
        }
        BannerAdOn();
    }
    public void BannerAdOn()
    {
        bannerAd.OnAdButtonClicked();
    }
}