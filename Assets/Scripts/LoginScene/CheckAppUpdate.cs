using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Google.Play.AppUpdate;
using Google.Play.Common;



public class CheckAppUpdate : MonoBehaviour
{
    // 유니티 플러그 인앱과 Play API 간 통신을 처리하는 클래스
    AppUpdateManager appUpdateManager = null;

    private void Awake()
    {
        // 업데이트 하고 얼마나 지났는지 경과 일수 확인하는 방법
        // var stalenessDays = appUpdateInfoOperation.GetResult();

        StartCoroutine(CheckForUpdate());
    }

    /// <summary>
    /// Google.Play.AppUdate 클래스
    /// AppUdateInfo : 앱의 업데이트 가용성 및 설치 진행률에 대한 정보
    /// AppUpdateManager : 앱 내에 업데이트를 요청하고 시작하는 작업
    /// AppUpdateOptions : AppUpdateType을 포함하여 앱 내 업데이트를 구성하는 데 사용되는 옵션
    /// AppUpdateRequest : 진행 중인 앱 내 업데이트를 모니터링하는 데 사용되는 사용자 지정 수율 명령
    /// </summary>
    /// <returns></returns>

    // 업데이트 사용 가능 여부를 확인
    IEnumerator CheckForUpdate()
    {

        yield return new WaitForSeconds(0.5f);

        appUpdateManager = new AppUpdateManager();
        PlayAsyncOperation<AppUpdateInfo, AppUpdateErrorCode> appUpdateInfoOperation;
        appUpdateInfoOperation = appUpdateManager.GetAppUpdateInfo();

        // 비동기 작업이 안료될 때까지 기다림
        yield return appUpdateInfoOperation;

        if(appUpdateInfoOperation.IsSuccessful)
        {
            // ApapUpdateManager.StartUpdate() 업데이트를 요청
            // 업데이트를 요정하기 전에 최신 AppUpdateInfo가 있는지 확인
            // UpdateAvailabillity. 앱 내 업데이트에 대한 가용성 정보

            var appUpdateInfoResult = appUpdateInfoOperation.GetResult();
            if(appUpdateInfoResult.UpdateAvailability == UpdateAvailability.UpdateAvailable)
            {
                Debug.Log("start updateAble");

                // 업데이트 사용 가능

                // 유연한 업데이트 처리
                //var appUpdateOptions = AppUpdateOptions.FlexibleAppUpdateOptions();

                // 즉시 업데이트 처리
                var appUpdateOptions = AppUpdateOptions.ImmediateAppUpdateOptions();

                // 지정된 업데이트 유형에 대해 앱 내 업데이트 시작

                var startUpdateRequest = appUpdateManager.StartUpdate(
                    // appUpdateInfoPeration.GetResult() 의 결과를 가져옴
                    appUpdateInfoResult
                    // 업데이트 처리 방식을 가져옴
                    , appUpdateOptions 
                    );

                while(!startUpdateRequest.IsDone)
                {
                    Debug.Log("startUpdateRequest.IsDon");

                    if(startUpdateRequest.Status == AppUpdateStatus.Pending)
                    {
                        // 업데이트 다운로드가 보류중 곧 처리 될 예정
                    }
                    else if(startUpdateRequest.Status == AppUpdateStatus.Downloading)
                    {
                        // 업데이트 다운도르가 진행 중 일 경우
                        Debug.Log("AppUpdateStatus.DownLoading");
                    }
                    else if (startUpdateRequest.Status == AppUpdateStatus.Downloaded)
                    {
                        // 업데이트가 완전히 끝난 경우
                        Debug.Log("AppUpdateStatus.Downloaded");
                    }
                    yield return null;
                }

                var result = appUpdateManager.CompleteUpdate();
                while(!result.IsDone) 
                {
                    yield return new WaitForEndOfFrame();
                }

                yield return (int)startUpdateRequest.Status;
            }
            else if(appUpdateInfoResult.UpdateAvailability == UpdateAvailability.UpdateNotAvailable)
            {
                Debug.Log("Not Update");
                yield return (int)UpdateAvailability.UpdateNotAvailable;
            }
            else
            {
                Debug.Log("Error");
                yield return (int)UpdateAvailability.Unknown;
            }
        }
        else
        {
            Debug.Log("Update Error");
        }
    }
}
