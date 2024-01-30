using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using TMPro;
using UnityEngine;

public class SlimeGameManager : MonoBehaviour
{
    private static SlimeGameManager instance = null;

    //Ǯ�� ������Ʈ ����
    [SerializeField]
    private GameObject slimePrefab;
    Queue<SlimePrefabScript> poolingObjectQueue = new Queue<SlimePrefabScript>();
    //Ǯ�� ������Ʈ ����

    //������Ʈ ���� ����
    private float[] sizes = new float[11];

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

    //�������
    public int score;
    private string filePath;
    private string encryKey = "qlalfqjsgh";
    //���� ����

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

    private void InitializeSphere(int initCount)// �ʱ� ���� ��
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

            Camera.main.GetComponent<CameraMoveScript>().GameOverCameraMove();
        }
    }
}
