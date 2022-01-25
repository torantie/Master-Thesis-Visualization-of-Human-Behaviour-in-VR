using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class NLPCalcRequester
{
    private const string DefaultUrl = "localhost:8001";

    public string RequestUrl { get; private set; }

    public NLPCalcRequester(string a_requestUrl = DefaultUrl)
    {
        RequestUrl = a_requestUrl;
    }


    /// <summary>
    /// https://forum.unity.com/threads/posting-json-through-unitywebrequest.476254/
    /// </summary>
    /// <param name="a_audioEventDataPoints"></param>
    /// <returns></returns>
    public IEnumerator Post(SpatialAudioEventDataPoint[] a_audioEventDataPoints, TextFileWriter a_textFileWriter)
    {
        var bodyJsonString = JsonArrayHelper.ToJson(a_audioEventDataPoints);
        UnityWebRequest request = CreatePostRequest(bodyJsonString);

        yield return request.SendWebRequest();

        Debug.Log("Status Code: " + request.responseCode);
        ProcessRequestResult(request, a_textFileWriter);
    }

    private void ProcessRequestResult(UnityWebRequest a_request, TextFileWriter a_textFileWriter)
    {
        try
        {
            switch (a_request.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                    Debug.LogWarningFormat("Result: {0} Error: {1}", a_request.result, a_request.error);
                    break;
                case UnityWebRequest.Result.DataProcessingError:
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogErrorFormat("Result: {0} Error: {1}", a_request.result, a_request.error);
                    break;
                case UnityWebRequest.Result.Success:
                    var responseText = a_request.downloadHandler.text;
                    Debug.LogFormat("Result: {0} Received: {1}", a_request.result, responseText);
                    var response = JsonArrayHelper.FromJson<NLPCalcResponse>(responseText);
                    foreach (var nlpCalcResponse in response)
                    {
                        var audioDataPoint = (SpatialAudioEventDataPoint)nlpCalcResponse;
                        foreach (var sentenceSentiment in nlpCalcResponse.sentenceSentiments)
                        {
                            var sentimentDataPoint = new SentimentEventDataPoint(audioDataPoint)
                            {
                                sentiment = sentenceSentiment.sentiment,
                                sentence = sentenceSentiment.sentence
                            };
                            a_textFileWriter.WriteToCSV(sentimentDataPoint);
                        }
                    }
                    a_textFileWriter.WriteToNLPCalc(response, true);
                    break;
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError(ex);
        }
    }

    /// <summary>
    /// https://forum.unity.com/threads/posting-json-through-unitywebrequest.476254/
    /// </summary>
    /// <param name="a_url"></param>
    /// <param name="a_bodyJsonString"></param>
    /// <returns></returns>
    private UnityWebRequest CreatePostRequest(string a_bodyJsonString)
    {
        var request = new UnityWebRequest(RequestUrl, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(a_bodyJsonString);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        return request;
    }
}
