using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using LitJson;
using Plc.Rpc;
using Plc.WebServerRequest;
using UnityEngine;
using UnityEngine.UI;

namespace Plc.Http
{
    public class HttpMgr : MonoBehaviour
    {
        //Get WebClient List to change team name
        public WebClientManage mWebClientManage;
        bool canChangeTeamName = false;
        
        string teamNameJson = "{'teamDatas':[{'teamName':'超能特战队1','GrmNumber':'30000014557'}," +
                          "{'teamName':'超能特战队2','GrmNumber':'30000842545'}]}";

        string testJson = "{'teamData':[{'teamName':'超能特战队','GrmNumber':'111111112222'},{'teamName': '超能特战队2','GrmNumber': '2223331312'}]}";

        string test = "";

        string ip;
        string teamNamePath = "/backend/sandbox/exlog?SID=123456&OP=UpdateTeamName";
        string attackEventUploadPath = "/backend/sandbox/exlog?SID=123456&OP=AttackEvent";

        private void Start()
        {
            Init();
            Debug.Log("qxn change data");
            //httpAttackEventUpload = GameObject.Find("HttpGetTeamNameData").GetComponent<HttpAttackEventUpload>();

        }
        IEnumerator ReadTxt()
        {
            WWW wIP = new WWW(Application.streamingAssetsPath + "/ip.txt");
            yield return wIP;
            ip = wIP.text;
            WWW wIs = new WWW(Application.streamingAssetsPath + "/isServer.txt");
            yield return wIs;
            if (wIs.text == "否")
            {
                httpAttackEventUpload.isServer = false;
                _httpReadTeamName.isServer = false;
            }
            else
            {
                httpAttackEventUpload.isServer = true;
                _httpReadTeamName.isServer = true;
            }
            StartCoroutine(_httpReadTeamName.ReadTeamName(ip, teamNamePath));
        }

        HttpReadTeamName _httpReadTeamName = new HttpReadTeamName();
        HttpAttackEventUpload httpAttackEventUpload = new HttpAttackEventUpload();
        #region  data and state init
        void Init()
        {
            canChangeTeamName = false;
            Invoke("DelayToSetState",0.5f);
            //获取战队名字
            StartCoroutine(ReadTxt());
            ////
            //ip = File.ReadAllText(Application.streamingAssetsPath + "/ip.txt");
            //teamNamePath= File.ReadAllText(Application.streamingAssetsPath + "/ip.txt");
            //attackEventUploadPath= File.ReadAllText(Application.streamingAssetsPath + "/ip.txt");
        }

        /// <summary>
        /// 保证刚开始的战队名称，变量等数据的初始化。
        /// </summary>
        void DelayToSetState()
        {
            canChangeTeamName = true;
            SetTeamName(teamNameJson);
        }
        #endregion

        #region Set TeamName by class TeamDataJsonParse  Jsondata GrmNumber
        /// <summary>
        /// 将战队名称匹配到对应的GRM设备
        /// </summary>
        /// <param name="_json"></param>
        public void SetTeamName(string _json)
        {

            TeamDataJson _teamJson = JsonMapper.ToObject<TeamDataJson>(_json);
            //Debug.LogError(_teamJson.teamDatas[0].teamName + " number : " + _teamJson.teamDatas[0].GrmNumber);
            foreach (var item in mWebClientManage.webClientList)
            {
                for (int i = 0; i < _teamJson.teamDatas.Count; i++)
                {
                    if (item.GrmNumber.Equals(_teamJson.teamDatas[i].GrmNumber))
                    {
                        item.TeamName = _teamJson.teamDatas[i].teamName;
                        Debug.LogError(item.TeamName);
                    }
                }
            }
        }

        /// <summary>
        /// auto send webserver enum value change event to http server
        /// </summary>
        /// <param name="_plcMsg"></param>
        public void AutoSendAttackEvent(PLCMsg _plcMsg,string _grmNumber)
        {
            Debug.LogError(" 发送数据到服务 Team Name : " + _plcMsg.threadType + " eSceneNameType:" + 
                      _plcMsg.eSceneNameType +" enumName:" + _plcMsg.enumName +
                      " enumValue:" + _plcMsg.enumValue);
            httpAttackEventUpload.AttackEvent(ip,attackEventUploadPath,_plcMsg.threadType, _grmNumber, _plcMsg.eSceneNameType.ToString(), _plcMsg.enumName, _plcMsg.enumValue);
            //httpAttackEventUpload.AttackEvent(ip,attackEventUploadPath,_plcMsg.threadType, "1111111111", _plcMsg.eSceneNameType.ToString(), _plcMsg.enumName, _plcMsg.enumValue);
        }

        #endregion
        
    }
}
