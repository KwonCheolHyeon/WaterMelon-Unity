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

    //Ǯ�� ������Ʈ ����
    [SerializeField]
    private GameObject slimePrefab;
    Queue<SlimePrefabScript> poolingObjectQueue = new Queue<SlimePrefabScript>();
    private Vector3 initSphereVec3 = new Vector3(0, 30, 0);
    //������Ʈ ���� ����
    private float[] sizes = new float[11];

    //���� ���� ����
    [SerializeField]
    private GameObject TongsMove;
    private bool isTongs = false;

    //���� ���� ����
    [SerializeField]
    private TextMeshProUGUI nowScoreText;
    [SerializeField]
    private TextMeshProUGUI hightScroreText;
    private int gameScore = 0;
    private int highScore = 0;

    //���� ���� ����
    private bool gameoverState = false;
    [SerializeField]
    private GameObject gameOverPanel;
    [SerializeField]
    private TextMeshProUGUI gameOverScoreText;

    //�������� ����
    [SerializeField]
    private GameObject stage;
    [SerializeField]
    private TextMeshProUGUI textShakeCount;
    private int shakeCount;

    //�޺� �ý��� ����
    [SerializeField]
    private GameObject comboImageObj;
    private float comboTime;
    private int comboCount;
    private bool isComboState;

    //���� ����
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

        //���� �� �̺�Ʈ�� ����� �ݴϴ�.
        SceneManager.sceneLoaded += LoadedsceneEvent;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
            GameOver();

        if (isComboState)
        {
            // �޺� ������ ��, comboTime�� ����
            comboTime -= Time.deltaTime;
            // comboTime�� 0 ���ϰ� �Ǹ�, �޺��� �ʱ�ȭ
            if (comboTime <= 0)
            {
                InItCombo();
            }
        }
    }

    // �� ���� �� ����
    private void LoadedsceneEvent(UnityEngine.SceneManagement.Scene _scene, LoadSceneMode _mode)
    {
        DataLoad();
    }


    public void InitializeSphere(int initCount)// �ʱ� ���� ��
    {
        for (int i = 0; i < initCount; i++)
        {
            poolingObjectQueue.Enqueue(CreateNewObject(0, sizes[0]));
        }
    }

    private SlimePrefabScript CreateNewObject(int _type, float _size)//Ǯ�� ������Ʈ �����ҽ� ���� �� �ʱ� ����
    {
        var newObj = Instantiate(slimePrefab).GetComponent<SlimePrefabScript>();
        newObj.GetComponent<SlimePrefabScript>().SettingSphere(_type, _size);
        newObj.gameObject.SetActive(false);
        newObj.transform.position = initSphereVec3;
        newObj.transform.SetParent(transform);
        newObj.GetComponent<MeshCollider>().enabled = false;
        return newObj;
    }

    public SlimePrefabScript GetObject(int _type)//������Ʈ �ҷ�����
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

    public SlimePrefabScript GetLevelUpObject(int _type)//�������ÿ��� �ҷ���
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

    public void ReturnObject(SlimePrefabScript obj)//������Ʈ ȸ��
    {
        obj.GetComponent<Rigidbody>().useGravity = true; //HideSphereObject ���� 2���� ������Ʈ���� �ٽ� ���ְ� active�� ��

        obj.GetComponent<MeshCollider>().enabled = false;
        obj.gameObject.SetActive(false);
        obj.transform.SetParent(Instance.transform);
        Instance.poolingObjectQueue.Enqueue(obj);
    }

    public void SphereBottomTrue()//�ٴڿ� ����� ��
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
            gameScore += GameScoreUp(type); // �޺� ���°� �ƴ� ���� �⺻ ������ �߰�
            comboCount += 1; // �޺� ī��Ʈ�� 1�� ����
            isComboState = true; // �޺� ���¸� Ȱ��
            comboTime = 3.0f; // �޺� Ÿ�̸Ӹ� 3�ʷ� ����
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
                Debug.LogError("Game Manager SetGameScore type ����");
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

    public void ShakeAdUiPanel() //���°� ������
    {
        rewardType = 0;
        adPannel.SetActive(true);
    }

    public void ChangeAdUiPanel()//�ٲٴ� �� ������
    {
        rewardType = 1;
        adPannel.SetActive(true);
    }

    public void UserAllowAd()//������ �����°��� ��� 
    {
        UnityAdsManager.Instance.RewardType(rewardType);
        UnityAdsManager.Instance.Request();
    }

    public void RewardShackStageCard() 
    {
        adPannel.SetActive(false);
        shakeCount = 1;//������ 0�̱� ������ 1�� �����.
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

            // PlayGamesPlatform �������忡 ���� �߰�
            PlayGamesPlatform.Instance.ReportScore(score, GPGSIds.leaderboard_slimescore, (bool success) => { });
            // ���� ���� ����
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
            Debug.Log("�ְ����� ���� ������ ���� �Ϸ�");
        }
    }

    private void DataLoad()
    {
        if (ES3.FileExists(saveFileName) && ES3.KeyExists(datakey, saveFileName))
        {

            scoreDatas = ES3.Load<GameScoreDatas>(datakey, saveFileName);
            highScore = scoreDatas.slimeHighScore;

            hightScroreText.text = highScore.ToString();

            Debug.Log("�ְ����� ���� ������ �ε� �Ϸ�");
        }
        else
        {
            scoreDatas = new GameScoreDatas();

            InitializeDefaultData();
            ScoreDataSave();

            scoreDatas = ES3.Load<GameScoreDatas>(datakey, saveFileName);
            Debug.Log("�ְ����� ���� ������ �ε� �Ϸ�");
        }
    }

    private void InitializeDefaultData()
    {
        scoreDatas.slimeHighScore = 0;
    }
}
