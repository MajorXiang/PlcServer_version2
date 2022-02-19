using UnityEngine;
using Plc.Data;
using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Plc.Rpc;

namespace Plc.WebServerRequest
{
    /// <summary>
    /// web client To deal with login and Get enumType for web server
    /// </summary>
    public partial class WebClient : MonoBehaviour
    {
        [SerializeField]
        private string sid = "";
        private Item item;
        [SerializeField]
        public EThreadType threadType = EThreadType.None;
        public Item Item { set => item = value; }
        public string GrmNumber;
        public void SetThreadType(int _number)
        {
            threadType = SceneAndThreadsData.GetType(_number);
        }

        #region ClientLogin 
        public void ClientLogin(object obj)
        {
            int _time = int.Parse(obj.ToString());
            Thread.Sleep(_time * 1000);
            var _GRM = item.GRM;
            string _PassWord = item.WebPass;
            if (_PassWord == "")
            {
                _PassWord = PlcKey.sKey;
            }
            byte[] data = Encoding.UTF8.GetBytes("GRM=" + _GRM + "\nPASS=" + _PassWord);
            string utf8Str = Encoding.UTF8.GetString(data);
            //注意Encoding.UTF8.GetString 与 Encoding.UTF8.GetBytes 的编码方式必须是一至的。
            //byte[] infos = Encoding.UTF8.GetBytes(input);
            //string utf8Str = Encoding.UTF8.GetString(infos);
            ZTHttpTool.Instance.Post(PlcKey.GetLoginMethodType(), utf8Str, LoginFinsh);
        }

        private void LoginFinsh(string _str)
        {
            List<string> striparr = _str.Split(new string[] { "\r\n" }, StringSplitOptions.None).ToList();
            striparr = striparr.Where(s => !string.IsNullOrEmpty(s)).ToList();
            if (striparr[0] == "OK")
            {
                sid = striparr[2].Replace("SID=", string.Empty);
                //Debug.Log("Get SID :" + sid);
                //登录成功后测试服务器数据
                GetEnumType(sid);
                //获取服务器状态局域网端不使用
                //GetWebServerStatus(SID);
            }
            else
            {
                Debug.LogError("服务器或者网络环境异常：" + _str);
            }
        }
        #endregion

        #region GetEnumType
        private void GetEnumType(string _SID)
        {
            ZTHttpTool.Instance.Post(PlcKey.GetEnumMethodType(_SID), "NTRPGC", GetEnumFinsh);
        }

        private void GetEnumFinsh(string _str)
        {
            List<string> striparr = _str.Split(new string[] { "\r\n" }, StringSplitOptions.None).ToList();
            striparr = striparr.Where(s => !string.IsNullOrEmpty(s)).ToList();
            if (striparr[0] == "OK")
            {
                //Debug.Log("Get Value number : " + striparr[1]);
                Debug.Log(_str);
                ParseEnumType(striparr);
            }
            else
            {
                Debug.LogError(_str);
            }
        }

        /// <summary>
        ///获取S开头的变量名称。作为枚举变量
        /// </summary>
        /// <param name="_striparr"></param>
        void ParseEnumType(List<string> _striparr)
        {
            List<EnumData> enumList = new List<EnumData>();
            foreach (var item in _striparr)
            {
                if (item.Substring(0, 1).Equals("S"))
                {
                    //Debug.Log("TRUE : " + item);
                    var enumData = new EnumData();
                    enumData.sceneNumber = int.Parse(CalculateTools.MidStrEx(item, "S", "N"));
                    //enumData.enumName = CalculateTools.MidStrEx(item,"N",",");
                    enumData.enumName = CalculateTools.GetStringToEndStr (item,",");
                    //读写权限的判定
                    //if (enumData.enumName.Contains("R"))
                    //最后一个字符R = RealOnly; W = ReadAndWrite;
                    if(enumData.enumName.Substring(enumData.enumName.Length - 1, 1) == "R") 
                    {
                        enumData.permissions = "ReadOnly";
                        SetSceneTypeValue(enumData);
                        enumList.Add(enumData);
                    }
                    else
                    {
                        enumData.permissions = "ReadAndWrite";
                        SetSceneTypeValue(enumData);
                        writeEnumList.Add(enumData);
                        //Debug.Log(enumData.DebugSelf());
                    }
                    //SetSceneTypeValue(enumData);
                    //Debug.Log(enumData.DebugSelf());
                    //enumList.Add(enumData);
                }
            }
            GetEnumDataFromServerFinsh(enumList);
        }

        void SetSceneTypeValue(EnumData _enumData)
        {
            _enumData.eSceneNameType = SceneAndThreadsData.GetSceneType(_enumData.sceneNumber);
        }

        void GetEnumDataFromServerFinsh(List<EnumData> _enumList)
        {
            //ReadDataFromWebServer();
            //Auto init web server enum value;
            InitWebServerEnumValue(_enumList);
            //Auto send enum type to all rpc client;
            //RpcServer.Instance.serverSendMsg.WebServerSetEnumTypeData(enumListCache);
        }

        #endregion

        #region GetWebServerStatus 未使用 局域网版本不使用
        private void GetWebServerStatus(string _sid)
        {
            ZTHttpTool.Instance.Post(PlcKey.GetStatusMethodType(_sid), "null", GetWebServerStatusFinsh);
        }

        private void GetWebServerStatusFinsh(string _str)
        {
            List<string> striparr = _str.Split(new string[] { "\r\n" }, StringSplitOptions.None).ToList();
            striparr = striparr.Where(s => !string.IsNullOrEmpty(s)).ToList();
            if (striparr[0] == "OK")
            {
                Debug.Log(_str);
                Debug.Log("当前有 ： " + striparr[1] + "个web客户端");
                if (striparr[2] == "2")
                {
                    Debug.Log("模块状态：正常工作");
                }
                Debug.Log("模块登录时间 ： " + striparr[3]);
                Debug.Log("最近活动时间 ： " + striparr[4]);
                Debug.Log("ClientIP ： " + striparr[5]);
            }
            else
            {
                Debug.LogError(_str);
            }
        }
        #endregion

    }
}