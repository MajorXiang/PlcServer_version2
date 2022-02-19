using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Threading;
using System.Text;
using System.IO;
using System;
using Plc.Http;

public class HttpReadTeamName
{
    HttpMgr httpMgr;
    public bool isServer = false;
    string baseurl = "from SteamAsset/ip.txt";
    string readUrl = "/backend/sandbox/exlog?SID=123456&OP=UpdateTeamName";
    // string readUrl ="/backend/sandbox/exlog?SID=123456&OP=AttackEvent&teamName=dddd&grmIp=dssdsd&SceneType=retrt&Value=fdfd";    
    public IEnumerator ReadTeamName(string _baseurl,string _readUrl)
    {
        baseurl = _baseurl;
        readUrl = _readUrl;
        httpMgr = GameObject.Find("HttpGetTeamNameData").GetComponent<HttpMgr>();
        //ZTHttpTool.Instance.PostRequestTest(baseurl, readUrl, "", ReadTeamNameFinsh);
        while (true)
        {
            if(isServer)
                PostRequestTest(baseurl,readUrl, "", ReadTeamNameFinsh);
            yield return new WaitForSeconds(3f);
            // Debug.Log(_readUrl);
        }
    }
    string teamNameJson;
    string lastTimeData;
    private void ReadTeamNameFinsh(string _str)
    {
        //Debug.LogError(_str);
        
        teamNameJson = "";
        string[] strArray = _str.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
        for (int i = 0; i < strArray.Length; i++)
        {
            if (i == 1 || i == 2) { }
            else
                teamNameJson += strArray[i];
        }
        httpMgr.SetTeamName(teamNameJson);
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



