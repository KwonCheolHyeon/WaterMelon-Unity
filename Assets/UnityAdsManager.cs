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
        // 오브젝트가 파괴될 때 이벤트에서 함수를 제거하여 메모리 누수를 방지합니다.
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
                Debug.LogError("보상 오류 RewardOn");
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
                Debug.LogError("보상 오류 RewardOn");
            }
        }
    }
}
