using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using System.Text;
using GooglePlayGames;

public class SpaceGameManager : MonoBehaviour
{
    private static SpaceGameManager instance = null;
    //풀링 오브젝트 관련
    [SerializeField]
    private GameObject spaceObjects;
    private Queue<SpaceSphereScript> poolingObjectQueues;
    private Vector3 initSphereVec3 = new Vector3(0, 16, 0);
    //풀링 오브젝트 관련

    //오브젝트 세팅 관련
    private float[] sizes = new float[11];
    [SerializeField]
    private Mesh[] meshes;
    [SerializeField]
    private GameObject collEffect;

    //집게 세팅 관련
    [SerializeField]
    private GameObject TongsMove;
    private bool isTongs = false;
    //집게 세팅 관련

    //게임 점수 관련
    [SerializeField]
    private TextMeshProUGUI nowScoreText;
    [SerializeField]
    private TextMeshProUGUI hightScroreText;
    private int gameScore = 0;
    //게임 점수 관련

    //게임 오버 관련
    private bool gameoverState = false;
    [SerializeField]
    private GameObject gameOverPanel;

    //카메라 관련
    private SpaceCameraMoveScript cameraScr;
    //

    //스테이지 흔들기
    [SerializeField]
    private GameObject stage;
    [SerializeField]
    private TextMeshProUGUI textShakeCount;
    private int shakeCount;
    //스테이지 흔들기

    //콤보 시스템 관련
    private float comboTime;
    private int comboCount;
    private bool isComboState;
    //콤보 시스템 관련


    public int Score // Property
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

    public static SpaceGameManager Instance
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

    public Mesh[] GetSapceMesh() 
    {
        return meshes;
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

        float prevSize = 0.1f;
        for (int index = 0; index < 11; index++)
        {
            sizes[index] = prevSize + 0.0225f;//space sphere이랑 연동 되어야 함
            prevSize = sizes[index];
        }

        gameOverPanel.SetActive(false);
        cameraScr = Camera.main.GetComponent<SpaceCameraMoveScript>();
        InitializeSphere(18);
    }

    void Start()
    {
        comboCount = 1;
        comboTime = 0;
        isComboState = false;
        shakeCount = 2;
        nowScoreText.text = 0 + "";

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

    private void InitializeSphere(int initCount)
    {
        poolingObjectQueues = new Queue<SpaceSphereScript>();
        for (int i = 0; i < initCount; i++)
        {
            poolingObjectQueues.Enqueue(CreateNewObject(0, sizes[0]));
        }
    }

    private SpaceSphereScript CreateNewObject(int _type, float _size)
    {
        SpaceSphereScript newObj = Instantiate(spaceObjects).AddComponent<SpaceSphereScript>();
        newObj.SpaceMeshSetting();
        newObj.SettingSphere(_type, _size);
        newObj.gameObject.SetActive(false);
        newObj.transform.position = initSphereVec3;
        newObj.transform.SetParent(transform);
        newObj.GetComponent<SphereCollider>().enabled = false;
        newObj.SetCollEffect(collEffect);

        return newObj;
    }
    public SpaceSphereScript GetObject(int _type)//오브젝트 불러오기
    {
        if (gameoverState)
            return null;

        float _size = SpaceGameManager.instance.sizes[_type];
        if (poolingObjectQueues.Count > 0)
        {
            var obj = Instance.poolingObjectQueues.Dequeue();
            obj.GetComponent<SpaceSphereScript>().SettingSphere(_type, _size);
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

    public SpaceSphereScript GetLevelUpObject(int _type)//레벨업시에만 불러옴
    {
        float _size = SpaceGameManager.instance.sizes[_type];
        if (Instance.poolingObjectQueues.Count > 0)
        {
            var obj = Instance.poolingObjectQueues.Dequeue();
            obj.GetComponent<SpaceSphereScript>().SettingSphere(_type, _size);
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

    public void ReturnObject(SpaceSphereScript obj, int _type)
    {

        obj.GetComponent<Rigidbody>().useGravity = true; //HideSphereObject 꺼준 2개의 컴포넌트들을 다시 켜주고 active를 끔

        obj.GetComponent<SphereCollider>().enabled = false;
        obj.gameObject.SetActive(false);
        obj.transform.SetParent(Instance.transform);
        poolingObjectQueues.Enqueue(obj);
    }

    public void SphereBottomTrue()//바닥에 닿았을 때
    {
        TongsMove.GetComponent<SpaceTongsMoveScript>().NextSphererInforMation();
    }

    public void SetGameScore(int type)
    {
        if (isComboState)
        {
            comboTime = 3.0f;

            if (comboCount > 4)
            {
                gameScore += GameScoreUp(type) * 5;
            }
            else if (comboCount == 4)
            {
                gameScore += GameScoreUp(type) * comboCount;
            }
            else if (comboCount == 3)
            {
                gameScore += GameScoreUp(type) * comboCount;
            }
            else if (comboCount == 2)
            {
                gameScore += GameScoreUp(type) * comboCount;
            }
            else 
            {
                Debug.LogError("ComBoCount 가 0입니다.");
            }
            comboCount += 1;
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
        isComboState = false;
        comboCount = 1;
        comboTime = 3.0f;
    }
    private int GameScoreUp(int _type) 
    {
        int gameScoreUp = 0 ;
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

    public void GameOver()
    {
        if (gameoverState == false)
        {
            GameObject.Find("Canvas").gameObject.SetActive(false);

            gameoverState = true;
            gameOverPanel.SetActive(true);
            cameraScr.GameOverCameraMove();

            // PlayGamesPlatform 리더보드에 점수 추가
            PlayGamesPlatform.Instance.ReportScore(gameScore, GPGSIds.achievement_score, (bool success) => { });
        }
    }

    public void CameraShake()
    {
        cameraScr.TriggerCameraShake();
    }

    public void TriggerStageShake()
    {


        if(shakeCount == 0) 
        {
            return;
        }
        shakeCount--;
        textShakeCount.text = shakeCount.ToString();

        float duration = 0.3f;
        float magnitude = 0.2f;
        StartCoroutine(ShakeObject(duration, magnitude));
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
}
