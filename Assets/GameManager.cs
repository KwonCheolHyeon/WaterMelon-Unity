using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance = null;

    //Ǯ�� ������Ʈ ����
    [SerializeField]
    private GameObject sphereObjectPrefab;
    Queue<SpherePrefabScript> poolingObjectQueue = new Queue<SpherePrefabScript>();
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
    private int gameScore = 0;
    //���� ���� ����

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

    public static GameManager Instance
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

        float prevSize = 0.05f;
        for (int index = 0; index < 9; index++)
        {
            sizes[index] = prevSize + 0.25f;
            prevSize = sizes[index];
        }
        sizes[9] = 2.6f;
        sizes[10] = 3.0f;

        InitializeSphere(10);
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void InitializeSphere(int initCount)// �ʱ� ���� ��
    {
        for (int i = 0; i < initCount; i++)
        {
            poolingObjectQueue.Enqueue(CreateNewObject(0, sizes[0]));
        }
    }

    private SpherePrefabScript CreateNewObject(int _type,float _size)//Ǯ�� ������Ʈ �����ҽ� ���� �� �ʱ� ����
    {
        var newObj = Instantiate(sphereObjectPrefab).GetComponent<SpherePrefabScript>();
        newObj.GetComponent<SpherePrefabScript>().SettingSphere(_type, _size);
        newObj.gameObject.SetActive(false);
        newObj.transform.SetParent(transform);
        return newObj;
    }

    public SpherePrefabScript GetObject(int _type)//������Ʈ �ҷ�����
    {
        float _size = GameManager.instance.sizes[_type];
        if (Instance.poolingObjectQueue.Count > 0)
        {
            var obj = Instance.poolingObjectQueue.Dequeue();
            obj.GetComponent<SpherePrefabScript>().SettingSphere(_type, _size);
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

    public void ReturnObject(SpherePrefabScript obj)//������Ʈ ȸ��
    {
        obj.gameObject.SetActive(false);
        obj.transform.SetParent(Instance.transform);
        Instance.poolingObjectQueue.Enqueue(obj);
    }

    public void SphereBottomTrue()//�ٴڿ� ����� ��
    {
        TongsMove.GetComponent<TongsMoveScript>().SettingSphereMove();
    }
   
}
