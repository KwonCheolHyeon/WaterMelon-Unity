using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using System.Text;

public class GameManager : MonoBehaviour
{
    private static GameManager instance = null;
    //Ǯ�� ������Ʈ ����
    [SerializeField]
    private GameObject sphereObjectPrefab;
    Queue<SpherePrefabScript> poolingObjectQueue = new Queue<SpherePrefabScript>();
    private Vector3 initSphereVec3 = new Vector3(0,16,0);
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

    //ī�޶� ����
    private CameraMoveScript cameraScr;
    //

    //�������� ����
    [SerializeField]
    private GameObject stage;


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

        float prevSize = 0.8f;
        for (int index = 0; index < 11; index++)
        {
            sizes[index] = prevSize + 0.2f;
            prevSize = sizes[index];
        }

        gameOverPanel.SetActive(false);
        cameraScr = Camera.main.GetComponent<CameraMoveScript>();
        InitializeSphere(100);
    }

    void Start()
    {
        nowScoreText.text = 0 + "";

        filePath = Application.persistentDataPath + "/GameData.txt";
        Load();
        hightScroreText.text =  score + "";
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.F)) 
        {
            TriggerStageShake();
            //Save();
        }
    }

    void Save()
    {
        try
        {
            if (score < gameScore) 
            {
                score = gameScore;
            }
            string scoreString = score.ToString();
            string encryptedScore = EncryptData(scoreString, encryKey);
            File.WriteAllText(filePath, encryptedScore);

        }
        catch (Exception ex)
        {
            Debug.LogError("Save error: " + ex.Message);
        }
    }

    void Load()
    {
        try
        {
            if (File.Exists(filePath))
            {
                string encryptedScore = File.ReadAllText(filePath);
                string decryptedScore = DecryptData(encryptedScore, encryKey);

                if (int.TryParse(decryptedScore, out int loadedScore))
                {
                    score = loadedScore;
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Load error: " + ex.Message);
        }
    }
    private string EncryptData(string text, string key)
    {
        byte[] clearBytes = Encoding.Unicode.GetBytes(text);
        using (Aes encryptor = Aes.Create())
        {
            // Generate a random salt
            byte[] salt = new byte[16];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(salt);
            }

            Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(key, salt);
            encryptor.Key = pdb.GetBytes(32);
            encryptor.IV = pdb.GetBytes(16);

            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(clearBytes, 0, clearBytes.Length);
                    cs.Close();
                }
                // Prepend the salt to the encrypted data for storage
                byte[] encryptedData = ms.ToArray();
                byte[] encryptedDataWithSalt = new byte[salt.Length + encryptedData.Length];
                Array.Copy(salt, 0, encryptedDataWithSalt, 0, salt.Length);
                Array.Copy(encryptedData, 0, encryptedDataWithSalt, salt.Length, encryptedData.Length);
                return Convert.ToBase64String(encryptedDataWithSalt);
            }
        }
    }

    private string DecryptData(string encryptedTextWithSalt, string key)
    {
        byte[] encryptedDataWithSalt = Convert.FromBase64String(encryptedTextWithSalt);
        byte[] salt = new byte[16];
        Array.Copy(encryptedDataWithSalt, 0, salt, 0, salt.Length);
        byte[] encryptedData = new byte[encryptedDataWithSalt.Length - salt.Length];
        Array.Copy(encryptedDataWithSalt, salt.Length, encryptedData, 0, encryptedData.Length);

        using (Aes encryptor = Aes.Create())
        {
            Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(key, salt);
            encryptor.Key = pdb.GetBytes(32);
            encryptor.IV = pdb.GetBytes(16);

            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(encryptedData, 0, encryptedData.Length);
                    cs.Close();
                }
                return Encoding.Unicode.GetString(ms.ToArray());
            }
        }
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
        newObj.transform.position = initSphereVec3;
        newObj.transform.SetParent(transform);
        newObj.GetComponent<SphereCollider>().enabled = false;
        return newObj;
    }

    public SpherePrefabScript GetObject(int _type)//������Ʈ �ҷ�����
    {
        if (gameoverState)
            return null;

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

    public SpherePrefabScript GetLevelUpObject(int _type)//�������ÿ��� �ҷ���
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
        
        obj.GetComponent<Rigidbody>().useGravity = true; //HideSphereObject ���� 2���� ������Ʈ���� �ٽ� ���ְ� active�� ��
       
        obj.GetComponent<SphereCollider>().enabled = false;
        obj.gameObject.SetActive(false);
        obj.transform.SetParent(Instance.transform);
        Instance.poolingObjectQueue.Enqueue(obj);
    }

    public void SphereBottomTrue()//�ٴڿ� ����� ��
    {
        TongsMove.GetComponent<TongsMoveScript>().NextSphererInforMation();
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

        nowScoreText.text = gameScore    + "";

    }

    public void GameOver() 
    {
        if(gameoverState == false) 
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
