using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Networking;
using System.Text;
using System.Net;
using System.IO;

/// <summary>
/// Http Request SDK 
/// </summary>
public class ZTHttpTool : MonoBehaviour
{
    public static ZTHttpTool Instance;
    [HideInInspector]
    private string baseUrl = "http://127.0.0.1:7080";

    Dictionary<string, string> requestHeader = new Dictionary<string, string>();  //  header

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        //http header 的内容
        //requestHeader.Add("Content-Type", "application/json");
        requestHeader.Add("Content-Type", "text/plain;charset=UTF-8");
        string datas = File.ReadAllText(Application.streamingAssetsPath+"/WebServerIP.txt");
        baseUrl = datas;
    }

    public void Get(string methodName, Action<string> callback)
    {
        StartCoroutine(GetRequest(methodName, callback));
    }
    public IEnumerator GetRequest(string methodName, Action<string> callback)
    {
        string url = baseUrl + methodName;
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            //设置header
            foreach (var v in requestHeader)
            {
                webRequest.SetRequestHeader(v.Key, v.Value);
            }
            yield return webRequest.SendWebRequest();

            if (webRequest.isHttpError || webRequest.isNetworkError)
            {
                Debug.LogError(webRequest.error + "\n" + webRequest.downloadHandler.text);
                if (callback != null)
                {
                    callback(null);
                }
            }
            else
            {
                if (callback != null)
                {
                    callback(webRequest.downloadHandler.text);
                }
            }
        }
    }

    //jsonString 为json字符串，post提交的数据包为json
    public void Post(string methodName, string jsonString, Action<string> callback)
    {
        //StartCoroutine(PostRequest(methodName, jsonString, callback));
        PostRequestTest(baseUrl, methodName, jsonString, callback);
    }
    public IEnumerator PostRequest(string methodName, string jsonString, Action<string> callback)
    {
        string url = baseUrl + methodName;
        Debug.Log("Send MethodName : " + methodName + " url : " + url);
        // Debug.Log(string.Format("url:{0} postData:{1}",url,jsonString));
        using (UnityWebRequest webRequest = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonString);
            webRequest.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
            webRequest.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();

            foreach (var v in requestHeader)
            {
                webRequest.SetRequestHeader(v.Key, v.Value);
            }
            yield return webRequest.SendWebRequest();

            if (webRequest.isHttpError || webRequest.isNetworkError)
            {
                Debug.LogError(webRequest.error + "\n" + webRequest.downloadHandler.text);
                if (callback != null)
                {
                    callback(webRequest.error);
                }
            }
            else
            {
                if (callback != null)
                {
                    callback(webRequest.downloadHandler.text);
                }
            }
        }
    }
    public void PostRequestTest(string _baseUrl, string methodName, string jsonString, Action<string> callback)
    {
        string url = _baseUrl + methodName;
        // Debug.Log("Send MethodName : " + methodName + " url : " + url);
        string result = "";
        HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
        req.Method = "POST";
        req.ContentType = "text/plain;charset=UTF-8";
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonString);
        req.ContentLength = bodyRaw.Length;
        using (Stream reqStream = req.GetRequestStream())
        {
            reqStream.Write(bodyRaw, 0, bodyRaw.Length);
            reqStream.Close();
        }
        HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
        Stream stream = resp.GetResponseStream();
        using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
        {
            result = reader.ReadToEnd();
        }
        callback(result);
    }

}