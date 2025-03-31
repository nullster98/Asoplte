using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/*public class GoogleSheetLoader : MonoBehaviour
{
    [SerializeField] private string sheetUrl;

    public Action<SheetEventList> OnDataLoaded;

    public void StartDownload()
    {
        StartCoroutine(DownloadAndParseJson());
    }

    private IEnumerator DownloadAndParseJson()
    {
        UnityWebRequest www = UnityWebRequest.Get(sheetUrl);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("다운로드 실패: " + www.error);
        }
        else
        {
            string rawJson = www.downloadHandler.text;

            // 수동으로 감싸기 (data로 감싸기)
            string wrappedJson = $"{{\"data\": {rawJson}}}";

            SheetEventList parsedData = JsonUtility.FromJson<SheetEventList>(wrappedJson);
            OnDataLoaded?.Invoke(parsedData);
        }
    }
}*/