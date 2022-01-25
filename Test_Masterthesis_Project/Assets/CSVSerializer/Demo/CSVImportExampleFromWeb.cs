using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
#if UNITY_5_3_OR_NEWER
using UnityEngine.Networking;
#else
using UnityEngine.Experimental.Networking;
#endif
using UnityEditor;
#endif

#if UNITY_EDITOR
/// <summary>
/// Edit to hide obsolete functions.
/// </summary>
public class CSVImportExampleFromWeb : Editor
{
    [MenuItem("Tools/CSV Serializer/DownloadExample")]
    [System.Obsolete]
    static void Init()
    {
        string url = "https://docs.google.com/spreadsheets/d/e/2PACX-1vTzdUCZ3VJYDjTY8IJcv7lBXYoi_ek4ZYqslgNSY46FNEaBPiWnHytGT6kg7r0nxa0QTRYs1SaHRdYg/pub?gid=0&single=true&output=csv";
        string assetfile = "Assets/CSVSerializer/demo/f1ranking2018.asset";

        StartCorountine(DownloadAndImport(url, assetfile));
    }

    [System.Obsolete]
    static IEnumerator DownloadAndImport(string url, string assetfile)
    {
        //WWWForm form = new WWWForm();
        //UnityWebRequest www = UnityWebRequest.Post(url, form);
        UnityWebRequest www = UnityWebRequest.Get(url);
        yield return www.Send();

        while (www.isDone == false)
        {
            yield return new WaitForEndOfFrame();
        }

        if (www.error != null)
        {
            Debug.Log("UnityWebRequest.error:" + www.error);
        }
        else if (www.downloadHandler.text == "" || www.downloadHandler.text.IndexOf("<!DOCTYPE") != -1)
        {
            Debug.Log("Uknown Format:" + www.downloadHandler.text);
        }
        else
        {
            ImportRankingData(www.downloadHandler.text, assetfile);
#if DEBUG_LOG || UNITY_EDITOR
            Debug.Log("Imported Asset: " + assetfile);
#endif
        }
    }

    static void ImportRankingData(string text, string assetfile)
    {
        List<string[]> rows = CSVSerializer.ParseCSV(text);
        if (rows != null)
        {
            RankingData gm = AssetDatabase.LoadAssetAtPath<RankingData>(assetfile);
            if (gm == null)
            {
                gm = new RankingData();
                AssetDatabase.CreateAsset(gm, assetfile);
            }
            gm.m_Items = CSVSerializer.Deserialize<RankingData.Item>(rows);

            EditorUtility.SetDirty(gm);
            AssetDatabase.SaveAssets();
        }
    }

    // coroutine for unity editor

    static void StartCorountine(IEnumerator routine)
    {
        _coroutine.Add(routine);
        if (_coroutine.Count == 1)
            EditorApplication.update += ExecuteCoroutine;
    }
    static List<IEnumerator> _coroutine = new List<IEnumerator>();
    static void ExecuteCoroutine()
    {
        for (int i = 0; i < _coroutine.Count;)
        {
            if (_coroutine[i] == null || !_coroutine[i].MoveNext())
                _coroutine.RemoveAt(i);
            else
                i++;
        }
        if (_coroutine.Count == 0)
            EditorApplication.update -= ExecuteCoroutine;
    }
}
#endif