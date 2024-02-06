using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderKeywordFilter;
using UnityEngine;
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
            // ��� ���� �ε带 �����մϴ�.
            bannerAd.LoadBanner();

            // ����: BannerAd ��ũ��Ʈ ������ OnBannerLoaded �ݹ鿡��
            // ���� �ε�Ǹ� �ڵ����� ǥ�õǵ��� �����ؾ� �մϴ�.
            // ��, LoadBanner() �޼��� ȣ�� ���Ŀ� ������ ShowBannerAd()�� ȣ���� �ʿ䰡 ����� �մϴ�.
            // ���� �ε尡 �Ϸ�Ǹ� OnBannerLoaded ������ ���� ǥ�õǵ��� �ϼ���.
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
        if (sceneType == 0)
        {
            if (rewardType == 0)
            {

            }
            else if (rewardType == 1)
            {

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

            }
            else if (rewardType == 1)
            {

            }
            else 
            {
                Debug.LogError("���� ���� RewardOn");
            }
        }
    }
}
