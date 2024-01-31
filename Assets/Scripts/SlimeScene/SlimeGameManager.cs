using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using TMPro;
using UnityEditor.SceneManagement;
using UnityEngine;

public class SlimeGameManager : MonoBehaviour
{
    private static SlimeGameManager instance = null;

    //풀링 오브젝트 관련
    [SerializeField]
    private GameObject slimePrefab;
    Queue<SlimePrefabScript> poolingObjectQueue = new Queue<SlimePrefabScript>();

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

    private void InitializeSphere(int initCount)// 초기 설정 용
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

        switch (type)
        {
            case 0:
                gameScore += 2;
                break;
            case 1:
                gameScore += 6;
                break;
            case 2:
                gameScore += 12;
                break;
            case 3:
                gameScore += 20;
                break;
            case 4:
                gameScore += 30;
                break;
            case 5:
                gameScore += 42;
                break;
            case 6:
                gameScore += 56;
                break;
            case 7:
                gameScore += 72;
                break;
            case 8:
                gameScore += 90;
                break;
            case 9:
                gameScore += 110;
                break;
            case 10:
                gameScore += 132;
                break;
            default:
                Debug.LogError("Game Manager SetGameScore type 오류");
                break;
        }

        nowScoreText.text = gameScore + "";

    }

    public void TriggerStageShake()
    {
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
