using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class LoginScenarioScript : MonoBehaviour
{
    [SerializeField]
    private SceneNames nextSceneName;

    [SerializeField]
    private ProgressScript progress;

    private string log;

    private void Awake()
    {
        SystemSetup();
    }

    private void SystemSetup()
    {
        ClickLogIn();

        // Ȱ��ȭ���� ���� ���¿����� ������ ��� ����
        Application.runInBackground = true;

        // �ػ� ���� (9.16, 1440x2960)
        //int width = Screen.width;
        //int height = (int)(Screen.height * 16.0f / 9);
        //Screen.SetResolution(width, height, true);

        // ȭ���� ������ �ʵ��� ����
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        progress.Play(OnAfterProgress);
    }

    public void OnAfterProgress()
    {
        UtisScript.LoadScene(nextSceneName);
    }

    public void ClickLogIn()
    {
        GPGSBinderScript.Instance.Login((success, localUser) =>
            log = $"{success}, {localUser.userName}, {localUser.id}, {localUser.state}, {localUser.underage}");
    }

    public void ClickLogOut()
    {
        GPGSBinderScript.Instance.Logout();
    }
}
