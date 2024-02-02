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
    //Ǯ�� ������Ʈ ����
    [SerializeField]
    private GameObject spaceObjects;
    private Queue<SpaceSphereScript> poolingObjectQueues;
    private Vector3 initSphereVec3 = new Vector3(0, 16, 0);
    //Ǯ�� ������Ʈ ����

    //������Ʈ ���� ����
    private float[] sizes = new float[11];
    [SerializeField]
    private Mesh[] meshes;
    //������Ʈ ���� ����

    //���� ���� ����
    [SerializeField]
    private GameObject TongsMove;
    private bool isTongs = false;
    //���� ���� ����

    //���� ���� ����
    [SerializeField]
    private TextMeshProUGUI nowScoreText;
    [SerializeField]
    private TextMeshProUGUI hightScroreText;
    private int gameScore = 0;
    //���� ���� ����

    //���� ���� ����
    private bool gameoverState = false;
    [SerializeField]
    private GameObject gameOverPanel;

    //ī�޶� ����
    private SpaceCameraMoveScript cameraScr;
    //

    //�������� ����
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
            sizes[index] = prevSize + 0.0225f;//space sphere�̶� ���� �Ǿ�� ��
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
    public SpaceSphereScript GetObject(int _type)//������Ʈ �ҷ�����
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

    public SpaceSphereScript GetLevelUpObject(int _type)//�������ÿ��� �ҷ���
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

        obj.GetComponent<Rigidbody>().useGravity = true; //HideSphereObject ���� 2���� ������Ʈ���� �ٽ� ���ְ� active�� ��

        obj.GetComponent<SphereCollider>().enabled = false;
        obj.gameObject.SetActive(false);
        obj.transform.SetParent(Instance.transform);
        poolingObjectQueues.Enqueue(obj);
    }

    public void SphereBottomTrue()//�ٴڿ� ����� ��
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
                Debug.LogError("Game Manager SetGameScore type ����");
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
