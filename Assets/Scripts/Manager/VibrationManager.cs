using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public class VibrationDatas
{
    public bool isVibration;
    public Sprite vibOptionSprite;
}

// 모바일 진동 관리 매니저
public class VibrationManager : MonoBehaviour
{
    private VibrationDatas vibDatas;
    private string datakey = "VibDatas";
    private string saveFileName = "SaveVibrationFile.es3";


    public AndroidJavaClass unityPlayer;
    public AndroidJavaObject vibrator;
    public AndroidJavaObject currentActivity;
    public AndroidJavaClass vibrationEffectClass;
    public int defaultAmplitude;


    [SerializeField]
    private Button vibrationOptionButton;
    [SerializeField]
    private Sprite vibTrueSprite;
    [SerializeField]
    private Sprite vibFalseSprite;

    private bool isVir;

    private static VibrationManager instance;
    public static VibrationManager Instance
    {
        get
        {
            if (instance == null)
            {
                Debug.LogError("VibrationManager instance is not found!");
            }
            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);

        }
    }

    void OnEnable()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        vibrator = currentActivity.Call<AndroidJavaObject>("getSystemService", "vibrator");
        if (getSDKInt() >= 26) {
            vibrationEffectClass = new AndroidJavaClass("android.os.VibrationEffect");
            defaultAmplitude = vibrationEffectClass.GetStatic<int>("DEFAULT_AMPLITUDE");
        }
#endif
    }

    private void Start()
    {
        DataLoad();

        VirbrationImageChange();

        //시작 시 이벤트를 등록해 줍니다.
        SceneManager.sceneLoaded += LoadedsceneEvent;
    }

    public void VirbrationImageChange()
    {
        if (isVir == true)
        {
            vibrationOptionButton.GetComponent<Image>().sprite = vibTrueSprite;
        }
        else
        {
            vibrationOptionButton.GetComponent<Image>().sprite = vibFalseSprite;
        }

        VibrationDataSave();
    }

    // Menu_Obj.Setting_Image.Vibration_Button.OnClick()에서 참조
    public void VibrationOptionChange()
    {
        if (isVir == true)
        {
            isVir = false;
        }
        else
        {
            isVir = true;
        }
        VirbrationImageChange();
    }

    // 씬 변경 시 실행
    private void LoadedsceneEvent(UnityEngine.SceneManagement.Scene _scene, LoadSceneMode _mode)
    {
        SetVibrationObject();

        DataLoad();
    }

    public void DataLoad()
    {
        if (ES3.FileExists(saveFileName) && ES3.KeyExists(datakey, saveFileName))
        {

            vibDatas = ES3.Load<VibrationDatas>(datakey, saveFileName);
            isVir = vibDatas.isVibration;

            Debug.Log("진동설정 데이터 로드 완료");
        }
        else
        {
            vibDatas = new VibrationDatas();

            InitializeDefaultData();
            VibrationDataSave();

            vibDatas = ES3.Load<VibrationDatas>(datakey, saveFileName);
            Debug.Log("진동설정 데이터 로드 완료");
        }
    }

    private void InitializeDefaultData()
    {
        isVir = true;
    }

    public void VibrationDataSave()
    {
        vibDatas.isVibration = isVir;

        ES3.Save(datakey, vibDatas, saveFileName);
        Debug.Log("진동설정 데이터 저장 완료");
    }

    private void SetVibrationObject()
    {
        // 씬 변경시 각 씬에 있는 VibratonButton을 찾아서 vibrationOptionButton에 넣어준다.
        vibrationOptionButton = GameObject.Find("Canvas").transform.Find("Menu_Obj").
            transform.Find("Setting_Image").transform.Find("SoundGroup").transform.Find("Vibration_Button").GetComponent<Button>();

        // 가져온 버튼의 onClick 이벤트에 VibrationOptionChange() 함수 생성
        vibrationOptionButton.onClick.AddListener(VibrationOptionChange);

        if (isVir)
            vibrationOptionButton.GetComponent<Image>().sprite = vibTrueSprite;
        else
            vibrationOptionButton.GetComponent<Image>().sprite = vibFalseSprite;

    }

    //Works on API > 25
    public void CreateOneShot(long milliseconds)
    {
        if (isVir)
        {
            if (isAndroid())
            {
                //If Android 8.0 (API 26+) or never use the new vibrationeffects
                if (getSDKInt() >= 26)
                {
                    CreateOneShot(milliseconds, defaultAmplitude);
                }
                else
                {
                    OldVibrate(milliseconds);
                }
            }
            //If not android do simple solution for now
            else
            {
                Handheld.Vibrate();
            }
        }
    }

    public void CreateOneShot(long milliseconds, int amplitude)
    {

        if (isAndroid())
        {
            //If Android 8.0 (API 26+) or never use the new vibrationeffects
            if (getSDKInt() >= 26)
            {
                CreateVibrationEffect("createOneShot", new object[] { milliseconds, amplitude });
            }
            else
            {
                OldVibrate(milliseconds);
            }
        }
        //If not android do simple solution for now
        else
        {
            Handheld.Vibrate();
        }
    }

    //Works on API > 25
    public void CreateWaveform(long[] timings, int repeat)
    {
        //Amplitude array varies between no vibration and default_vibration up to the number of timings

        if (isAndroid())
        {
            //If Android 8.0 (API 26+) or never use the new vibrationeffects
            if (getSDKInt() >= 26)
            {
                CreateVibrationEffect("createWaveform", new object[] { timings, repeat });
            }
            else
            {
                OldVibrate(timings, repeat);
            }
        }
        //If not android do simple solution for now
        else
        {
            Handheld.Vibrate();
        }
    }

    public void CreateWaveform(long[] timings, int[] amplitudes, int repeat)
    {
        if (isAndroid())
        {
            //If Android 8.0 (API 26+) or never use the new vibrationeffects
            if (getSDKInt() >= 26)
            {
                CreateVibrationEffect("createWaveform", new object[] { timings, amplitudes, repeat });
            }
            else
            {
                OldVibrate(timings, repeat);
            }
        }
        //If not android do simple solution for now
        else
        {
            Handheld.Vibrate();
        }

    }

    //Handels all new vibration effects
    private void CreateVibrationEffect(string function, params object[] args)
    {

        AndroidJavaObject vibrationEffect = vibrationEffectClass.CallStatic<AndroidJavaObject>(function, args);
        vibrator.Call("vibrate", vibrationEffect);
    }

    //Handles old vibration effects
    private void OldVibrate(long milliseconds)
    {
        vibrator.Call("vibrate", milliseconds);
    }
    private void OldVibrate(long[] pattern, int repeat)
    {
        vibrator.Call("vibrate", pattern, repeat);
    }

    public bool HasVibrator()
    {
        return vibrator.Call<bool>("hasVibrator");
    }

    public bool HasAmplituideControl()
    {
        if (getSDKInt() >= 26)
        {
            return vibrator.Call<bool>("hasAmplitudeControl"); //API 26+ specific
        }
        else
        {
            return false; //If older than 26 then there is no amplitude control at all
        }

    }

    public void Cancel()
    {
        if (isAndroid())
            vibrator.Call("cancel");
    }

    private int getSDKInt()
    {
        if (isAndroid())
        {
            using (var version = new AndroidJavaClass("android.os.Build$VERSION"))
            {
                return version.GetStatic<int>("SDK_INT");
            }
        }
        else
        {
            return -1;
        }

    }

    private bool isAndroid()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
	    return true;
#else
        return false;
#endif
    }

}
