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
        // 활성화되지 않은 상태에서도 게임이 계속 진행
        Application.runInBackground = true;

        // 해상도 설정 (9.16, 1440x2960)
        int width = Screen.width;
        int height = (int)(Screen.height * 16.0f / 9);
        Screen.SetResolution(width, height, true);

        // 화면이 꺼지지 않도록 설정
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }

    public void OnClickNextScene()
    {
        UtisScript.LoadScene(nextSceneName);
    }
}
