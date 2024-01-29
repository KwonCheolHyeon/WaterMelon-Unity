using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoginScenarioScript : MonoBehaviour
{
    [SerializeField]
    private SceneNames nextSceneName;

    private void Awake()
    {
        SystemSetup();
    }

    private void SystemSetup()
    {
        // Ȱ��ȭ���� ���� ���¿����� ������ ��� ����
        Application.runInBackground = true;

        // �ػ� ���� (9.16, 1440x2960)
        int width = Screen.width;
        int height = (int)(Screen.height * 16.0f / 9);
        Screen.SetResolution(width, height, true);

        // ȭ���� ������ �ʵ��� ����
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }

    public void OnClickNextScene()
    {
        UtisScript.LoadScene(nextSceneName);
    }
}
