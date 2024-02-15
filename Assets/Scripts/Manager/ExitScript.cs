using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;


public class ExitScript : MonoBehaviour
{
    [SerializeField]
    SceneNames sceneName;

    [SerializeField]
    private TextMeshProUGUI scoreText;

    // 3개의 GamePlayScene의 Menu_Obj.ButtonGroupExit_Button에서 참조
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

    // Menu_Obj.ButtonGroup.Replay_Button에서 참조
    public void RePlay()
    {
        Scene scene = SceneManager.GetActiveScene();
        UtisScript.LoadScene(scene.name);
    }

    public void ShowAllReaderBoard()
    {
        GPGSBinderScript.Instance.ShowAllLeaderboardUI();
    }


}
