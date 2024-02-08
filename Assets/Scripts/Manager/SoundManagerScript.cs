using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class SoundDatas
{
    public bool BGMPlay;
    public bool SFXPlay;
}

public class SoundManagerScript : MonoBehaviour
{
    // ���� ���� ������ ������ class
    private SoundDatas soundDatas;
    public SoundDatas GetSoundDatas() { return soundDatas; }

    private string datakey = "SoundDatas";
    private string saveFileName = "SaveSoundFile.es3";

    public AudioMixer audioMixer;

    private bool isBGMplay = true;
    private bool isSFXplay = true;

    private float soundVolume = 0.7f;

    private Button BGMButton;
    private Button SFXButton;

    [SerializeField]
    private Sprite playBGMSprite;
    [SerializeField]
    private Sprite playNotBGMSprite;
    [SerializeField]
    private Sprite playSFXSprite;
    [SerializeField]
    private Sprite playNotSFXSprite;

    [SerializeField]
    private GameObject SFXAudioresource;
    [SerializeField]
    private GameObject BGMAudioresource;

    [SerializeField]
    public SFXResource[] SFX;
    [SerializeField]
    public BGMResource[] BGM;

    private AudioSource BGMFromChild;
    private AudioSource SFXFromChild;

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
    public void PlaySFXSound(string _soundName, float _volume = 1.0f)
    {
        if(SFXFromChild.volume != 0)
        {
            foreach (var sfx in SFX)
            {
                if (sfx.SoundName == _soundName)
                {
                    SFXFromChild.clip = sfx.audioClip;
                    SFXFromChild.volume = _volume;
                    SFXFromChild.Play();
                    return;
                }
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
        PlayBGMSound("TestBGM");

        SetSoundUIObject();

        DataLoad();

        //���� �� �̺�Ʈ�� ����� �ݴϴ�.
        SceneManager.sceneLoaded += LoadedsceneEvent;
    }

    // �� ���� �� ����
    private void LoadedsceneEvent(UnityEngine.SceneManagement.Scene _scene, LoadSceneMode _mode)
    {
        Debug.Log(_scene.name + "���� ����Ǿ����ϴ�.");

        SetSoundUIObject();

        // ������ ���� �´� BGM ���
        SetSceneSpecificBGM();

        DataLoad();
    }

    public void DataLoad()
    {
        if (ES3.FileExists(saveFileName) && ES3.KeyExists(datakey, saveFileName))
        {

            soundDatas = ES3.Load<SoundDatas>(datakey, saveFileName);
            isSFXplay = soundDatas.SFXPlay;
            isBGMplay = soundDatas.BGMPlay;

            if (isBGMplay)
            {
                BGMFromChild.volume = soundVolume;
                BGMButton.GetComponent<Image>().sprite = playBGMSprite;
            }
            else if (!isBGMplay)
            {
                BGMFromChild.volume = 0;
                BGMButton.GetComponent<Image>().sprite = playNotBGMSprite;
            }

            if (isSFXplay)
            {
                SFXFromChild.volume = 1;
                SFXButton.GetComponent<Image>().sprite = playSFXSprite;
            }
            else if (!isSFXplay)
            {
                SFXFromChild.volume = 0;
                SFXButton.GetComponent<Image>().sprite = playNotSFXSprite;
            }

            Debug.Log("���� ���� ������ �ε� �Ϸ�");
        }
        else
        {
            soundDatas = new SoundDatas();

            InitializeDefaultData();
            SoundDataSave();

            soundDatas = ES3.Load<SoundDatas>(datakey, saveFileName);
            Debug.Log("���� ���� ������ �ε� �Ϸ�");
        }
    }

    private void InitializeDefaultData()
    {
        soundDatas.SFXPlay = isSFXplay;
        soundDatas.BGMPlay = isBGMplay;
    }

    public void SoundDataSave()
    {
        soundDatas.SFXPlay = isSFXplay;
        soundDatas.BGMPlay = isBGMplay;

        ES3.Save(datakey, soundDatas, saveFileName);

        Debug.Log("���� ���� ������ ���� �Ϸ�");
    }

    private void SetSoundUIObject()
    {
        BGMButton = GameObject.Find("Canvas").transform.Find("Menu_Obj").
            transform.Find("Setting_Image").transform.Find("SoundGroup").transform.Find("BGM_Button").GetComponent<Button>();

        BGMButton.onClick.AddListener(() => SetSound(BGMButton.name));

        SFXButton = GameObject.Find("Canvas").transform.Find("Menu_Obj").
            transform.Find("Setting_Image").transform.Find("SoundGroup").transform.Find("SFX_Button").GetComponent<Button>();

        SFXButton.onClick.AddListener(() => SetSound(SFXButton.name));
    }

    private void SetSceneSpecificBGM()
    {
        if(UtisScript.GetActiveScene() == SceneNames.SelectScene.ToString())
        {
            PlayBGMSound("TestBGM");
        }
        else if(UtisScript.GetActiveScene() == SceneNames.SlimeScene.ToString())
        {
            PlayBGMSound("SlimeBGM");
        }
        else if (UtisScript.GetActiveScene() == SceneNames.SpaceGameScene.ToString())
        {
            PlayBGMSound("SpaceBGM");
        }
    }

    public void SetSound(string _buttonName)
    {
        if(_buttonName == "BGM_Button")
        {
            if (isBGMplay)
            {
                isBGMplay = false;
                BGMFromChild.volume = 0;
                BGMButton.GetComponent<Image>().sprite = playNotBGMSprite;
            }
            else if (!isBGMplay)
            { 
                isBGMplay = true;
                BGMFromChild.volume = soundVolume;
                BGMButton.GetComponent<Image>().sprite = playBGMSprite;
            }
        }
        else if(_buttonName == "SFX_Button")
        {
            if (isSFXplay)
            {
                isSFXplay = false;
                SFXFromChild.volume = 0;
                SFXButton.GetComponent<Image>().sprite = playNotSFXSprite;
            }
            else if (!isSFXplay)
            {
                isSFXplay = true;
                SFXFromChild.volume = 1;
                SFXButton.GetComponent<Image>().sprite = playSFXSprite;
            }
        }

        SoundDataSave();
    }
}