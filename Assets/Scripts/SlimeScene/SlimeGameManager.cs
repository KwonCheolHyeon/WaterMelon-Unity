using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SlimeGameManager : MonoBehaviour
{
    private static SlimeGameManager instance = null;

    //풀링 오브젝트 관련
    [SerializeField]
    private GameObject slimePrefab;
    Queue<SlimePrefabScript> poolingObjectQueue = new Queue<SlimePrefabScript>();
    private Vector3 initSphereVec3 = new Vector3(0, 16, 0);
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

    //게임 오버 관련
    private bool gameoverState = false;
    [SerializeField]
    private GameObject gameOverPanel;

    //스테이지 흔들기
    [SerializeField]
    private GameObject stage;
    [SerializeField]
    private TextMeshProUGUI textShakeCount;
    private int shakeCount;

    //콤보 시스템 관련
    private float comboTime;
    private int comboCount;
    private bool isComboState;
    //콤보 시스템 관련

    public int score;


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
        comboCount = 1;
        comboTime = 0;
        isComboState = false;
        shakeCount = 2;
        nowScoreText.text = 0 + "";

    }

    // Update is called once per frame
    void Update()
    {
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
        if (shakeCount == 0)
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

    public void GameOver()
    {
        if (gameoverState == false)
        {
            gameoverState = true;
            gameOverPanel.SetActive(true);

            Camera.main.GetComponent<CameraMoveScript>().GameOverCameraMove();
        }
    }
}
