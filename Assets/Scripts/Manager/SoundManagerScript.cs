using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class SoundDatas
{
    public float masterSoundValue;
    public float BGMSoundValue;
    public float SFXSoundValue;

    public bool isCompleteTutorial;
}

public class SoundManagerScript : MonoBehaviour
{
    // ���� ���� ������ ������ class
    private SoundDatas soundDatas;
    public SoundDatas GetSoundDatas() { return soundDatas; }

    //private string datakey = "SoundDatas";
    //private string saveFileName = "SaveSoundFile.es3";

    public AudioMixer audioMixer;

    public Slider MasterSlider;
    public Slider BGMSlider;
    public Slider SFXSlider;

    [SerializeField]
    private GameObject slider;

    GameObject tabObj;

    // ===========================================
    // ���� ���ҽ� ����

    [SerializeField]
    private GameObject SFXAudioresource;
    [SerializeField]
    private GameObject BGMAudioresource;

    [SerializeField]
    public SFXResource[] SFX;
    [SerializeField]
    public BGMResource[] BGM;

    private AudioSource SFXFromChild;
    private AudioSource BGMFromChild;

    void GetSFXresource()
    {
        SFXFromChild = SFXAudioresource.GetComponent<AudioSource>();
    }

    void GetBGMResource()
    {
        BGMFromChild = BGMAudioresource.GetComponent<AudioSource>();
    }

    [Serializable]
    public struct SFXResource
    {
        [SerializeField]
        public string SoundName;
        [SerializeField]
        public AudioClip audioClip;
    }
    [Serializable]
    public struct BGMResource
    {
        [SerializeField]
        public string SoundName;
        [SerializeField]
        public AudioClip audioClip;
    }

    // BGM ���
    public void PlayBGMSound(string _soundName)
    {
        foreach (var bgm in BGM)
        {
            if (bgm.SoundName == _soundName)
            {
                BGMFromChild.clip = bgm.audioClip;
                BGMFromChild.Play();
                return;
            }
        }
        Debug.Log(_soundName + "���带 ã�� ���߽��ϴ�.");
    }

    // �Ͻ�����
    public void PauseBGMSound()
    {
        if (BGMFromChild.isPlaying)
        {
            BGMFromChild.Pause();
        }
    }

    // ����
    public void StopBGMSound()
    {
        if (BGMFromChild.isPlaying)
        {
            BGMFromChild.Stop();
        }
    }

    // SFX ���
    public void PlaySFXSound(string _soundName)
    {
        foreach (var sfx in SFX)
        {
            if (sfx.SoundName == _soundName)
            {
                SFXFromChild.clip = sfx.audioClip;
                SFXFromChild.Play();
                return;
            }
        }
        Debug.Log(_soundName + "���带 ã�� ���߽��ϴ�.");
    }

    public void PauseSFXSound()
    {
        if (SFXFromChild.isPlaying)
        {
            BGMFromChild.Pause();
        }
    }

    public void StopSFXSound()
    {
        if (SFXFromChild.isPlaying)
        {
            BGMFromChild.Stop();
        }
    }

    private static SoundManagerScript instance = null;
    public static SoundManagerScript Instance
    {
        get
        {
            if (null == instance)
            {
                //Debug.Log("�����̳� �Ŵ����� Null�Դϴ�.");
                return null;
            }
            return instance;
        }
    }

    private void Awake()
    {
        if (null == instance)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }

        GetSFXresource();
        GetBGMResource();
    }

    private void Start()
    {
        DataLoad();

        //���� �� �̺�Ʈ�� ����� �ݴϴ�.
        SceneManager.sceneLoaded += LoadedsceneEvent;
    }

    void Update()
    {
        SetSound(MasterSlider, "Master");
        SetSound(BGMSlider, "BGM");
        SetSound(SFXSlider, "SFX");

        if (Input.GetKeyDown(KeyCode.A))
        {
            SoundDataSave();
        }
    }

    // �� ���� �� ����
    private void LoadedsceneEvent(Scene _scene, LoadSceneMode _mode)
    {
        Debug.Log(_scene.name + "���� ����Ǿ����ϴ�.");

        SetSliderObject(_scene);

        // ���忡 �ʿ��� ������ �ҷ�����
        MasterSlider = slider.transform.GetChild(0).GetComponent<Slider>();
        BGMSlider = slider.transform.GetChild(1).GetComponent<Slider>();
        SFXSlider = slider.transform.GetChild(2).GetComponent<Slider>();

        DataLoad();
    }

    public void DataLoad()
    {
        //if (ES3.FileExists(saveFileName) && ES3.KeyExists(datakey, saveFileName))
        //{

        //    soundDatas = ES3.Load<SoundDatas>(datakey, saveFileName);
        //    MasterSlider.value = soundDatas.masterSoundValue;
        //    SFXSlider.value = soundDatas.SFXSoundValue;
        //    BGMSlider.value = soundDatas.BGMSoundValue;
        //    Debug.Log("���� ������ �ε� �Ϸ�");
        //}
        //else
        //{
        //    soundDatas = new SoundDatas();

        //    InitializeDefaultData();
        //    SoundDataSave();

        //    soundDatas = ES3.Load<SoundDatas>(datakey, saveFileName);
        //    Debug.Log("���� ������ �ε� �Ϸ�");
        //}
    }

    private void InitializeDefaultData()
    {
        soundDatas.masterSoundValue = MasterSlider.value;
        soundDatas.SFXSoundValue = SFXSlider.value;
        soundDatas.BGMSoundValue = BGMSlider.value;

        soundDatas.isCompleteTutorial = false;
    }

    public void SoundDataSave()
    {
        soundDatas.masterSoundValue = MasterSlider.value;
        soundDatas.SFXSoundValue = SFXSlider.value;
        soundDatas.BGMSoundValue = BGMSlider.value;

        //ES3.Save(datakey, soundDatas, saveFileName);
    }

    private void SetSliderObject(UnityEngine.SceneManagement.Scene _scene)
    {
        if (_scene.name == "StoreScene")
        {
            slider = GameObject.Find("Canvas").transform.Find("Setting_Tab").
                transform.Find("SettingPage").transform.Find("Slider").gameObject;
        }
        else if (_scene.name == "TitleScene")
        {
            slider = GameObject.Find("Title_Canvas").transform.Find("SettingTab").
                transform.Find("SettingPage").transform.Find("Slider").gameObject;
        }
        else if (_scene.name == "ItemStoreScene")
        {
            slider = GameObject.Find("Canvas").transform.Find("Setting_Tab").
                transform.Find("SettingPage").transform.Find("Slider").gameObject;
        }
        else if (_scene.name == "TutorialStoreScene")
        {
            slider = GameObject.Find("Canvas").transform.Find("Setting_Tab").
                transform.Find("SettingPage").transform.Find("Slider").gameObject;
        }
    }

    public void SetSound(Slider slider, string mixerGroup)
    {
        float sound = slider.value;

        // Slider�ּҰ����� ������ ���Ұŷ� ����
        if (sound == -40.0f)
        {
            audioMixer.SetFloat(mixerGroup, -80);
        }
        else
        {
            audioMixer.SetFloat(mixerGroup, sound);
        }
    }
}