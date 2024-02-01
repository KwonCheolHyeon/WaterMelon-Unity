using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitScript : MonoBehaviour
{
    [SerializeField]
    SceneNames sceneName;

    // 3개의 GamePlayScene - Exit_Button에서 참조
    public void FromSelectScene()
    {
        UtisScript.LoadScene(sceneName);
    }

    // SelectScene - Exit_Button에서 참조
    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
   UnityEngine.Application.Quit();
#endif
    }

    public void RePlay()
    {
        UtisScript.LoadScene("SlimeScene");
    }
}
