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

    // 3���� GamePlayScene�� Menu_Obj.ButtonGroupExit_Button���� ����
    public void FromSelectScene()
    {
        UtisScript.LoadScene(sceneName);
    }

    // SelectScene - Exit_Button���� ����
    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
   UnityEngine.Application.Quit();
#endif
    }

    // Menu_Obj.ButtonGroup.Replay_Button���� ����
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
