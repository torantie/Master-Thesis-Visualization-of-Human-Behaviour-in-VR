using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class CSVImportExample : ScriptableObject
{
    [System.Serializable]
    public class Sample
    {
        public int year;
        public string make;
        public string model;
        public string description;
        public float price;
    }
    public Sample[] m_Sample;
}

#if UNITY_EDITOR
public class CSVImportExamplePostprocessor : AssetPostprocessor
{
    static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        foreach (string str in importedAssets)
        {
            if (str.IndexOf("/sample.csv") != -1)
            {
                TextAsset data = AssetDatabase.LoadAssetAtPath<TextAsset>(str);
                string assetfile = str.Replace(".csv", ".asset");
                CSVImportExample gm = AssetDatabase.LoadAssetAtPath<CSVImportExample>(assetfile);
                if (gm == null)
                {
                    gm = new CSVImportExample();
                    AssetDatabase.CreateAsset(gm, assetfile);
                }

                gm.m_Sample = CSVSerializer.Deserialize<CSVImportExample.Sample>(data.text);

                EditorUtility.SetDirty(gm);
                AssetDatabase.SaveAssets();
#if DEBUG_LOG || UNITY_EDITOR
                Debug.Log("Reimported Asset: " + str);
#endif
            }
            if (str.IndexOf("/f1ranking2018.csv") != -1)
            {
                TextAsset data = AssetDatabase.LoadAssetAtPath<TextAsset>(str);
                string assetfile = str.Replace(".csv", ".asset");
                RankingData gm = AssetDatabase.LoadAssetAtPath<RankingData>(assetfile);
                if (gm == null)
                {
                    gm = new RankingData();
                    AssetDatabase.CreateAsset(gm, assetfile);
                }

                gm.m_Items = CSVSerializer.Deserialize<RankingData.Item>(data.text);

                EditorUtility.SetDirty(gm);
                AssetDatabase.SaveAssets();
#if DEBUG_LOG || UNITY_EDITOR
                Debug.Log("Reimported Asset: " + str);
#endif
            }
            if (str.IndexOf("/lan.csv") != -1)
            {
                TextAsset data = AssetDatabase.LoadAssetAtPath<TextAsset>(str);
                string assetfile = str.Replace(".csv", ".asset");
                LanguageStringData gm = AssetDatabase.LoadAssetAtPath<LanguageStringData>(assetfile);
                if (gm == null)
                {
                    gm = new LanguageStringData();
                    AssetDatabase.CreateAsset(gm, assetfile);
                }

                gm.m_Items = CSVSerializer.Deserialize<LanguageStringData.Item>(data.text);

                EditorUtility.SetDirty(gm);
                AssetDatabase.SaveAssets();
#if DEBUG_LOG || UNITY_EDITOR
                Debug.Log("Reimported Asset: " + str);
#endif
            }
            if (str.IndexOf("/const.csv") != -1)
            {
                TextAsset data = AssetDatabase.LoadAssetAtPath<TextAsset>(str);
                string assetfile = str.Replace(".csv", ".asset");
                ConstData gm = AssetDatabase.LoadAssetAtPath<ConstData>(assetfile);
                if (gm == null)
                {
                    gm = new ConstData();
                    AssetDatabase.CreateAsset(gm, assetfile);
                }

                ConstData readdata = CSVSerializer.DeserializeIdValue<ConstData>(data.text);
                EditorUtility.CopySerialized(readdata, gm);

                EditorUtility.SetDirty(gm);
                AssetDatabase.SaveAssets();
#if DEBUG_LOG || UNITY_EDITOR
                Debug.Log("Reimported Asset: " + str);
#endif
            }
        }
    }
}
#endif