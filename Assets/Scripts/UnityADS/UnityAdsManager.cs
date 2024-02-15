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
    private int sceneType = 0;//0 = ������ , 1 = ����
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
            DontDestroyOnLoad(gameObject); // �� ������Ʈ�� �� �ε� �� �ı����� �ʽ��ϴ�.

            // ���� �ε�� �� ȣ��� �Լ��� �̺�Ʈ�� ����մϴ�.
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else if (instance != this)
        {
            Destroy(gameObject); // �ߺ� �ν��Ͻ��� �����մϴ�.
        }
    }

    // ���� �ε�� ������ ȣ��� �Լ��Դϴ�.
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("Loaded new scene: " + scene.name);

        if (scene.name == "SlimeScene" || scene.name == "SpaceGameScene")
        {
            // GameManager�� ã���ϴ�.
            gameManager = GameObject.Find("GameManager");

            // sceneType�� �����մϴ�. (0: ������ ��, 1: ���� ��)
            sceneType = scene.name == "SlimeScene" ? 0 : 1;

            // ��� ���� �ε� �� ǥ���մϴ�.
            LoadAndShowBannerAd();
            Advertisement.Banner.Hide();
        }
    }

    void OnDestroy()
    {
        // ������Ʈ�� �ı��� �� �̺�Ʈ���� �Լ��� �����Ͽ� �޸� ������ �����մϴ�.
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    private void LoadAndShowBannerAd()
    {
        if (bannerAd != null)
        {

        }
        else
        {
            Debug.LogError("BannerAd ������Ʈ�� �Ҵ���� �ʾҽ��ϴ�.");
        }
    }

    public void Request()
    {
        rewardedAds.LoadAd();
    }

    public void RewardOn()
    {
        if (sceneType == 0)//������
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
                Debug.LogError("���� ���� RewardOn");
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
                Debug.LogError("���� ���� RewardOn");
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
        if (sceneType == 0)//������
        {
            Debug.Log("������ �� ���� ����");
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