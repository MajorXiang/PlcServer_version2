using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Plc.Rpc;
using System;
using System.Threading;
using System.Net;
using System.IO;

public class Attack
{
    public string teamName;
    public string GrmNumber;
    public string SceneType;
    public string variableName;
    public string Value;
}
public class HttpAttackEventUpload
{
    string baseurl = "http://47.110.242.161:50002";
    string attackEventUrl = "/backend/sandbox/exlog?SID=123456&OP=AttackEvent&teamName=dddd&grmIp=dssdsd&SceneType=retrt&Value=fdfd";
    string jsonstring;
    public bool isServer = false;

    private void Update()
    {

    }

    public void AttackEvent(string _baseurl,string _attackEventUrl ,string teamName,string GrmNumber,string SceneType,string variableName,string Value)
    {
        baseurl = _baseurl;
        attackEventUrl = _attackEventUrl;
        Debug.Log(teamName + "/" + GrmNumber + "/" + SceneType + "/" + variableName + "/" + Value);
        //Dictionary<string, string> mydata = new Dictionary<string, string>();
        //mydata["teamName"] = teamName;
        //mydata["GrmNumber"] = GrmNumber;
        //mydata["SceneType"] = SceneType;
        //mydata["variableName"] = variableName;
        //mydata["Value"] = Value;
        Attack att = new Attack()
        {
            teamName=teamName,
            GrmNumber=GrmNumber,
            SceneType=SceneType,
            variableName=variableName,
            Value=Value
        };
        jsonstring = JsonUtility.ToJson(att);
        Debug.LogWarning(jsonstring);
        att = JsonUtility.FromJson<Attack>(jsonstring);
        Debug.LogWarning(att.teamName + "/" + att.GrmNumber + "/" + att.SceneType + "/" + att.variableName + "/" + att.Value);
        WebAttackEvent(jsonstring);
    }
    void WebAttackEvent(string _json)
    {
        if (isServer)
        {
            //正式时需要开启
            PostRequestTest(baseurl, attackEventUrl, _json, AttackEventFinsh);
        }
        
    }
    private void AttackEventFinsh(string _str)
    {
        Debug.LogWarning(_str);
        ServerCode serverCode = new ServerCode();
        serverCode = JsonUtility.FromJson<ServerCode>(_str);
        //if (serverCode.code == 1)
        //{
        //    int start = Environment.TickCount;
        //    while (Math.Abs(Environment.TickCount - start) < 3000f)//毫秒
        //    {
        //        //Debug.LogWarning("无聊的操作");
        //        //可执行某无聊的操作
        //    }
        //    Text_Test.str = "Error Code";
        //    WebAttackEvent(jsonstring);
        //}
    }
    //IEnumerator OnPostRequstAuth(string _json)
    //{
    //    //方法二
    //    Dictionary<string, string> hearder = new Dictionary<string, string>();
    //    hearder["Content-Type"] = "application/json";
    //    var jsonstring = JsonUtility.ToJson(_json);
    //    byte[] postBytes = Encoding.Default.GetBytes(jsonstring);
    //    WWW www = new WWW(baseurl + attackEventUrl, postBytes, hearder);
    //    yield return www;
    //    if (www.error != null)
    //    {
    //        Debug.Log("Erroe:" + www.error);
    //    }
    //    else
    //    {
    //        Debug.Log("Text:" + www.text);
    //        AttackEventFinsh(www.text);
    //    }
    //    yield return new WaitForSeconds(3f);
    //}

    public void PostRequestTest(string _baseUrl, string methodName, string jsonString, Action<string> callback)
    {
        string url = _baseUrl + methodName;
        //Debug.LogError("111111111 Send Json : " + jsonString + " url : " + url);
        string result = "";
        HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
        req.Method = "POST";
        req.ContentType = "application/json";
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonString);
        req.ContentLength = bodyRaw.Length;
        using (Stream reqStream = req.GetRequestStream())
        {
            reqStream.Write(bodyRaw, 0, bodyRaw.Length);
            reqStream.Close();
        }

        try
        {
            HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
            Stream stream = resp.GetResponseStream();
            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
            {
                result = reader.ReadToEnd();
            }
        }
        catch (Exception e)
        {
            Debug.Log(e);
            // throw;
        }
        callback(result);
    }
}
public class ServerCode
{
    public int code;
    public string msg;
}