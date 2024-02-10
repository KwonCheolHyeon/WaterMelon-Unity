using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Google.Play.AppUpdate;
using Google.Play.Common;



public class CheckAppUpdate : MonoBehaviour
{
    // ����Ƽ �÷��� �ξ۰� Play API �� ����� ó���ϴ� Ŭ����
    AppUpdateManager appUpdateManager = null;

    private void Awake()
    {
        // ������Ʈ �ϰ� �󸶳� �������� ��� �ϼ� Ȯ���ϴ� ���
        // var stalenessDays = appUpdateInfoOperation.GetResult();

        StartCoroutine(CheckForUpdate());
    }

    /// <summary>
    /// Google.Play.AppUdate Ŭ����
    /// AppUdateInfo : ���� ������Ʈ ���뼺 �� ��ġ ������� ���� ����
    /// AppUpdateManager : �� ���� ������Ʈ�� ��û�ϰ� �����ϴ� �۾�
    /// AppUpdateOptions : AppUpdateType�� �����Ͽ� �� �� ������Ʈ�� �����ϴ� �� ���Ǵ� �ɼ�
    /// AppUpdateRequest : ���� ���� �� �� ������Ʈ�� ����͸��ϴ� �� ���Ǵ� ����� ���� ���� ���
    /// </summary>
    /// <returns></returns>

    // ������Ʈ ��� ���� ���θ� Ȯ��
    IEnumerator CheckForUpdate()
    {

        yield return new WaitForSeconds(0.5f);

        appUpdateManager = new AppUpdateManager();
        PlayAsyncOperation<AppUpdateInfo, AppUpdateErrorCode> appUpdateInfoOperation;
        appUpdateInfoOperation = appUpdateManager.GetAppUpdateInfo();

        // �񵿱� �۾��� �ȷ�� ������ ��ٸ�
        yield return appUpdateInfoOperation;

        if(appUpdateInfoOperation.IsSuccessful)
        {
            // ApapUpdateManager.StartUpdate() ������Ʈ�� ��û
            // ������Ʈ�� �����ϱ� ���� �ֽ� AppUpdateInfo�� �ִ��� Ȯ��
            // UpdateAvailabillity. �� �� ������Ʈ�� ���� ���뼺 ����

            var appUpdateInfoResult = appUpdateInfoOperation.GetResult();
            if(appUpdateInfoResult.UpdateAvailability == UpdateAvailability.UpdateAvailable)
            {
                Debug.Log("start updateAble");

                // ������Ʈ ��� ����

                // ������ ������Ʈ ó��
                //var appUpdateOptions = AppUpdateOptions.FlexibleAppUpdateOptions();

                // ��� ������Ʈ ó��
                var appUpdateOptions = AppUpdateOptions.ImmediateAppUpdateOptions();

                // ������ ������Ʈ ������ ���� �� �� ������Ʈ ����

                var startUpdateRequest = appUpdateManager.StartUpdate(
                    // appUpdateInfoPeration.GetResult() �� ����� ������
                    appUpdateInfoResult
                    // ������Ʈ ó�� ����� ������
                    , appUpdateOptions 
                    );

                while(!startUpdateRequest.IsDone)
                {
                    Debug.Log("startUpdateRequest.IsDon");

                    if(startUpdateRequest.Status == AppUpdateStatus.Pending)
                    {
                        // ������Ʈ �ٿ�ε尡 ������ �� ó�� �� ����
                    }
                    else if(startUpdateRequest.Status == AppUpdateStatus.Downloading)
                    {
                        // ������Ʈ �ٿ���� ���� �� �� ���
                        Debug.Log("AppUpdateStatus.DownLoading");
                    }
                    else if (startUpdateRequest.Status == AppUpdateStatus.Downloaded)
                    {
                        // ������Ʈ�� ������ ���� ���
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
