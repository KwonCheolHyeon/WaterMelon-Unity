using GooglePlayGames;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameScoreDatas
{
    public int slimeHighScore;
    public int spaceHighScore;
}


public class SlimeGameManager : MonoBehaviour
{
    private static SlimeGameManager instance = null;

    //풀링 오브젝트 관련
    [SerializeField]
    private GameObject slimePrefab;
    Queue<SlimePrefabScript> poolingObjectQueue = new Queue<SlimePrefabScript>();
    private Vector3 initSphereVec3 = new Vector3(0, 30, 0);
    //오브젝트 세팅 관련
    private float[] sizes = new float[11];

    //집게 세팅 관련
    [SerializeField]
    private GameObject TongsMove;
    private bool isTongs = false;

    //게임 점수 관련
    [SerializeField]
    private TextMeshProUGUI nowScoreText;
    [SerializeField]
    private TextMeshProUGUI hightScroreText;
    private int gameScore = 0;
    private int highScore = 0;

    //게임 오버 관련
    private bool gameoverState = false;
    [SerializeField]
    private GameObject gameOverPanel;
    [SerializeField]
    private TextMeshProUGUI gameOverScoreText;

    //스테이지 흔들기
    [SerializeField]
    private GameObject stage;
    [SerializeField]
    private TextMeshProUGUI textShakeCount;
    private int shakeCount;

    //콤보 시스템 관련
    [SerializeField]
    private GameObject comboImageObj;
    private float comboTime;
    private int comboCount;
    private bool isComboState;

    //광고 관련
    [SerializeField]
    private GameObject adPannel;
    [SerializeField]
    private Button adButton;
    private int rewardType;


    //easy save
    private GameScoreDatas scoreDatas;
    public GameScoreDatas GetSoundDatas() { return scoreDatas; }

    private string datakey = "highScoreDatas";
    private string saveFileName = "SaveScoreFile.es3";


    public int score // Property
    {
        get { return gameScore; } // Get accessor
        set { gameScore = value; } // Set accessor
    }

    public bool TongState
    {
        get { return isTongs; }
        set { isTongs = value; }
    }

    public bool GameOverState
    {
        get { return gameoverState; }
        set { gameoverState = value; }
    }

    public static SlimeGameManager Instance
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
        if (null == instance)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }

        float prevSize = 1.5f;
        for (int index = 0; index < 11; index++)
        {
            sizes[index] = prevSize + 0.4f;
            prevSize = sizes[index];
        }

        gameOverPanel.SetActive(false);

        InitializeSphere(100);
    }
    void Start()
    {
        comboCount = 0;
        comboTime = 0;
        isComboState = false;
        shakeCount = 2;
        nowScoreText.text = 0 + "";
        rewardType = 0;

        adPannel.SetActive(false);
        adButton.onClick.AddListener(UserAllowAd);
        adButton.interactable = true;

        DataLoad(); 

        //시작 시 이벤트를 등록해 줍니다.
        SceneManager.sceneLoaded += LoadedsceneEvent;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
            GameOver();

        if (isComboState)
        {
            // 콤보 상태일 때, comboTime을 감소
            comboTime -= Time.deltaTime;
            // comboTime이 0 이하가 되면, 콤보를 초기화
            if (comboTime <= 0)
            {
                InItCombo();
            }
        }
    }

    // 씬 변경 시 실행
    private void LoadedsceneEvent(UnityEngine.SceneManagement.Scene _scene, LoadSceneMode _mode)
    {
        DataLoad();
    }


    public void InitializeSphere(int initCount)// 초기 설정 용
    {
        for (int i = 0; i < initCount; i++)
        {
            poolingObjectQueue.Enqueue(CreateNewObject(0, sizes[0]));
        }
    }

    private SlimePrefabScript CreateNewObject(int _type, float _size)//풀링 오브젝트 부족할시 생성 및 초기 설정
    {
        var newObj = Instantiate(slimePrefab).GetComponent<SlimePrefabScript>();
        newObj.GetComponent<SlimePrefabScript>().SettingSphere(_type, _size);
        newObj.gameObject.SetActive(false);
        newObj.transform.position = initSphereVec3;
        newObj.transform.SetParent(transform);
        newObj.GetComponent<MeshCollider>().enabled = false;
        return newObj;
    }

    public SlimePrefabScript GetObject(int _type)//오브젝트 불러오기
    {
        if (gameoverState)
            return null;

        float _size = SlimeGameManager.instance.sizes[_type];
        if (Instance.poolingObjectQueue.Count > 0)
        {
            var obj = Instance.poolingObjectQueue.Dequeue();
            obj.GetComponent<SlimePrefabScript>().SettingSphere(_type, _size);
            obj.transform.SetParent(null);
            obj.gameObject.SetActive(true);
            return obj;
        }
        else
        {
            var newObj = Instance.CreateNewObject(_type, _size);
            newObj.gameObject.SetActive(true);
            newObj.transform.SetParent(null);
            return newObj;
        }
    }

    public SlimePrefabScript GetLevelUpObject(int _type)//레벨업시에만 불러옴
    {
        float _size = SlimeGameManager.instance.sizes[_type];
        if (Instance.poolingObjectQueue.Count > 0)
        {
            var obj = Instance.poolingObjectQueue.Dequeue();
            obj.GetComponent<SlimePrefabScript>().SettingSphere(_type, _size);
            obj.transform.SetParent(null);
            obj.gameObject.SetActive(true);
            return obj;
        }
        else
        {
            var newObj = Instance.CreateNewObject(_type, _size);
            newObj.gameObject.SetActive(true);
            newObj.transform.SetParent(null);
            return newObj;
        }
    }

    public void ReturnObject(SlimePrefabScript obj)//오브젝트 회수
    {
        obj.GetComponent<Rigidbody>().useGravity = true; //HideSphereObject 꺼준 2개의 컴포넌트들을 다시 켜주고 active를 끔

        obj.GetComponent<MeshCollider>().enabled = false;
        obj.gameObject.SetActive(false);
        obj.transform.SetParent(Instance.transform);
        Instance.poolingObjectQueue.Enqueue(obj);
    }

    public void SphereBottomTrue()//바닥에 닿았을 때
    {
        TongsMove.GetComponent<SlimeTongsMoveScript>().NextSphererInforMation();
    }

    public void SetGameScore(int type)
    {
        if (isComboState)
        {
            comboTime = 3.0f;
            comboCount += 1;

            if (comboCount > 4)
            {
                gameScore += GameScoreUp(type) * 5;
            }
            else if (comboCount == 4)
            {
                gameScore += GameScoreUp(type) * 4;
            }
            else if (comboCount == 3)
            {
                gameScore += GameScoreUp(type) * 3;
            }
            else if (comboCount == 2)
            {
                gameScore += GameScoreUp(type) * 2;
            }


            if (comboImageObj.activeSelf == false)
                comboImageObj.SetActive(true);

            comboImageObj.GetComponent<ComboScript>().SetComboImage(comboCount);
        }
        else
        {
            gameScore += GameScoreUp(type); // 콤보 상태가 아닐 때는 기본 점수를 추가
            comboCount += 1; // 콤보 카운트를 1로 시작
            isComboState = true; // 콤보 상태를 활성
            comboTime = 3.0f; // 콤보 타이머를 3초로 설정
        }


        nowScoreText.text = gameScore + "";
    }

    private void InItCombo()
    {
        comboImageObj.SetActive(false);

        isComboState = false;
        comboCount = 0;
        comboTime = 3.0f;
    }
    private int GameScoreUp(int _type)
    {
        int gameScoreUp = 0;
        switch (_type)
        {
            case 0:
                gameScoreUp += 2;
                break;
            case 1:
                gameScoreUp += 6;
                break;
            case 2:
                gameScoreUp += 12;
                break;
            case 3:
                gameScoreUp += 20;
                break;
            case 4:
                gameScoreUp += 30;
                break;
            case 5:
                gameScoreUp += 42;
                break;
            case 6:
                gameScoreUp += 56;
                break;
            case 7:
                gameScoreUp += 72;
                break;
            case 8:
                gameScoreUp += 90;
                break;
            case 9:
                gameScoreUp += 110;
                break;
            case 10:
                gameScoreUp += 132;
                break;
            default:
                Debug.LogError("Game Manager SetGameScore type 오류");
                break;
        }
        return gameScoreUp;
    }

    public void TriggerStageShake()
    {
        if (shakeCount == 0 && adPannel.activeSelf == false)
        {
            ShakeAdUiPanel();
        }
        else 
        {
            TriggerStageShakeOn();
        }
    }

    private void TriggerStageShakeOn() 
    {
        shakeCount--;
        textShakeCount.text = shakeCount.ToString();

        float duration = 0.3f;
        float magnitude = 0.2f;
        StartCoroutine(ShakeObject(duration, magnitude));
    }

    public void ShakeAdUiPanel() //흔드는거 없을때
    {
        rewardType = 0;
        adPannel.SetActive(true);
    }

    public void ChangeAdUiPanel()//바꾸는 거 없을때
    {
        rewardType = 1;
        adPannel.SetActive(true);
    }

    public void UserAllowAd()//유저가 광고보는것을 허락 
    {
        UnityAdsManager.Instance.RewardType(rewardType);
        UnityAdsManager.Instance.Request();
    }

    public void RewardShackStageCard() 
    {
        adPannel.SetActive(false);
        shakeCount = 1;//어차피 0이기 때문에 1로 만든다.
        textShakeCount.text = shakeCount.ToString();
    }

    public void RewardChangeCard()
    {
        adPannel.SetActive(false);
        TongsMove.GetComponent<SlimeTongsMoveScript>().SetChangeCount();
    }


    public IEnumerator ShakeObject(float duration, float magnitude)
    {
        Vector3 originalPos = stage.transform.localPosition;
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            float x = UnityEngine.Random.Range(-1f, 1f) * magnitude;
            float y = UnityEngine.Random.Range(-1f, 1f) * magnitude;
            float z = UnityEngine.Random.Range(-1f, 1f) * magnitude;

            stage.transform.localPosition = new Vector3(originalPos.x + x, originalPos.y + y, originalPos.z + z);

            elapsed += Time.deltaTime;
            yield return null;
        }

        stage.transform.localPosition = originalPos;
    }

    public void GameOver()
    {
        if (gameoverState == false)
        {
            gameoverState = true;

            GameObject.Find("Canvas").gameObject.SetActive(false);
            gameOverPanel.SetActive(true);

            gameOverScoreText.text = "Score : " + score;

            Camera.main.GetComponent<CameraMoveScript>().GameOverCameraMove();

            // PlayGamesPlatform 리더보드에 점수 추가
            PlayGamesPlatform.Instance.ReportScore(score, GPGSIds.leaderboard_slimescore, (bool success) => { });
            // 최종 점수 저장
            ScoreDataSave();

            UnityAdsManager.Instance.GameOver();
        }
    }

    public void SaveGameOverScore() 
    {

    }

    private void ScoreDataSave()
    {
        if(scoreDatas.slimeHighScore < score)
        {
            scoreDatas.slimeHighScore = score;
            ES3.Save(datakey, scoreDatas, saveFileName);
            Debug.Log("최고점수 설정 데이터 저장 완료");
        }
    }

    private void DataLoad()
    {
        if (ES3.FileExists(saveFileName) && ES3.KeyExists(datakey, saveFileName))
        {

            scoreDatas = ES3.Load<GameScoreDatas>(datakey, saveFileName);
            highScore = scoreDatas.slimeHighScore;

            hightScroreText.text = highScore.ToString();

            Debug.Log("최고점수 설정 데이터 로드 완료");
        }
        else
        {
            scoreDatas = new GameScoreDatas();

            InitializeDefaultData();
            ScoreDataSave();

            scoreDatas = ES3.Load<GameScoreDatas>(datakey, saveFileName);
            Debug.Log("최고점수 설정 데이터 로드 완료");
        }
    }

    private void InitializeDefaultData()
    {
        scoreDatas.slimeHighScore = 0;
    }
}
