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
        if (scene.name == "SlimeScene")
        {
            gameManager = GameObject.Find("GameManager");
            sceneType = 0;
        }
        else if (scene.name == "SpaceGameScene") 
        {
            gameManager = GameObject.Find("GameManager");
            sceneType = 1;
        }
    }

    void OnDestroy()
    {
        // ������Ʈ�� �ı��� �� �̺�Ʈ���� �Լ��� �����Ͽ� �޸� ������ �����մϴ�.
        SceneManager.sceneLoaded -= OnSceneLoaded;
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
