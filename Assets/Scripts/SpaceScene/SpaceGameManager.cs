using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using System.Text;

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
    //오브젝트 세팅 관련

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
        nowScoreText.text = 0 + "";

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            TriggerStageShake();
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

    public void GameOver()
    {
        if (gameoverState == false)
        {
            gameoverState = true;
            gameOverPanel.SetActive(true);
            cameraScr.GameOverCameraMove();
        }
    }

    public void CameraShake()
    {
        cameraScr.TriggerCameraShake();
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
}
