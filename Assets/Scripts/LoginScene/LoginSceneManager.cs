using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class LoginSceneManager : MonoBehaviour
{
    string log;
    int score = 0;


    void OnGUI()
    {
        //GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one * 3);
        //
        //
        //if (GUILayout.Button("ClearLog"))
        //    log = "";

        //if (GUILayout.Button("Login"))
        //    GPGSBinderScript.Instance.Login((success, localUser) =>
        //    log = $"{success}, {localUser.userName}, {localUser.id}, {localUser.state}, {localUser.underage}");

        //if (GUILayout.Button("Logout"))
        //    GPGSBinderScript.Instance.Logout();

        //if (GUILayout.Button("SaveCloud"))
        //    GPGSBinderScript.Instance.SaveCloud("mysave", "want data", success => log = $"{success}");

        //if (GUILayout.Button("LoadCloud"))
        //    GPGSBinderScript.Instance.LoadCloud("mysave", (success, data) => log = $"{success}, {data}");

        //if (GUILayout.Button("DeleteCloud"))
        //    GPGSBinderScript.Instance.DeleteCloud("mysave", success => log = $"{success}");

        //if (GUILayout.Button("ShowAchievementUI"))
        //    GPGSBinderScript.Instance.ShowAchievementUI();

        //if (GUILayout.Button("UnlockAchievement_one"))
        //    GPGSBinderScript.Instance.UnlockAchievement(GPGSIds.achievement_score, success => log = $"{success}");

        //if (GUILayout.Button("UnlockAchievement_two"))
        //    GPGSBinderScript.Instance.UnlockAchievement(GPGSIds.achievement_two, success => log = $"{success}");

        //if (GUILayout.Button("IncrementAchievement_three"))
        //    GPGSBinderScript.Instance.IncrementAchievement(GPGSIds.achievement_three, 1, success => log = $"{success}");

        //if (GUILayout.Button("ShowAllLeaderboardUI"))
        //    GPGSBinderScript.Instance.ShowAllLeaderboardUI();

        //if (GUILayout.Button("ShowTargetLeaderboardUI_num"))
        //    GPGSBinderScript.Instance.ShowTargetLeaderboardUI(GPGSIds.leaderboard_num);

        //if (GUILayout.Button("ReportLeaderboard_num"))
        //    GPGSBinderScript.Instance.ReportLeaderboard(GPGSIds.leaderboard_num, 1000, success => log = $"{success}");

        //if (GUILayout.Button("LoadAllLeaderboardArray_num"))
        //    GPGSBinderScript.Instance.LoadAllLeaderboardArray(GPGSIds.leaderboard_num, scores =>
        //    {
        //        log = "";
        //        for (int i = 0; i < scores.Length; i++)
        //            log += $"{i}, {scores[i].rank}, {scores[i].value}, {scores[i].userID}, {scores[i].date}\n";
        //    });

        //if (GUILayout.Button("LoadCustomLeaderboardArray_num"))
        //    GPGSBinderScript.Instance.LoadCustomLeaderboardArray(GPGSIds.leaderboard_num, 10,
        //        GooglePlayGames.BasicApi.LeaderboardStart.PlayerCentered, GooglePlayGames.BasicApi.LeaderboardTimeSpan.Daily, (success, scoreData) =>
        //        {
        //            log = $"{success}\n";
        //            var scores = scoreData.Scores;
        //            for (int i = 0; i < scores.Length; i++)
        //                log += $"{i}, {scores[i].rank}, {scores[i].value}, {scores[i].userID}, {scores[i].date}\n";
        //        });

        //if (GUILayout.Button("IncrementEvent_event"))
        //    GPGSBinderScript.Instance.IncrementEvent(GPGSIds.event_event, 1);

        //if (GUILayout.Button("LoadEvent_event"))
        //    GPGSBinderScript.Instance.LoadEvent(GPGSIds.event_event, (success, iEvent) =>
        //    {
        //        log = $"{success}, {iEvent.Name}, {iEvent.CurrentCount}";
        //    });

        //if (GUILayout.Button("LoadAllEvent"))
        //    GPGSBinderScript.Instance.LoadAllEvent((success, iEvents) =>
        //    {
        //        log = $"{success}\n";
        //        foreach (var iEvent in iEvents)
        //            log += $"{iEvent.Name}, {iEvent.CurrentCount}\n";
        //    });

        //GUILayout.Label(log);
    }

    public void AddScore()
    {
        score += 1;
        // 리더보드 추가
        //GPGSBinderScript.Instance.ReportLeaderboard(GPGSIds.leaderboard_num, score, (bool success) => { });
    }
}