using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Threading;
using Plc.Rpc;
namespace Plc.WebServerRequest
{
    /// <summary>
    /// web client To deal with read and write for web server
    /// </summary>
    public partial class WebClient
    {
        public List<EnumData> enumListCache;
        public List<EnumData> writeEnumList = new List<EnumData>();
        private string enumInitValue = "1";
        private string stringCache = "";
        public bool SetEnumList = false;
        public bool isFirstToReadData = true;
        //request read data from webserver frequency  read time
        private int mreadDataFrequency = 1;
        public ESceneNameType eSceneNameType = ESceneNameType.None;//作为场景类型数据从.dat文件外部读取
        public int targetEnumCount = 0;
        public string TeamName = "";
        public bool InitWebServerEnumValueState = false;
        //init web server init enum value resend request frequency
        private int reSendWriteValueFrequency = 2;

        private bool clientWriteValue = false;

        private bool MatchGrmToSceneType = true;

        #region client Write enum value to web server 
        public void ClientWriteDataToWebServer(string _str)
        {
            clientWriteValue = true;
            ClientWriteEnumDataFromServer(sid, _str, ClientWriteEnumDataFinsh);
        }

        private void ClientWriteEnumDataFromServer(string _SID, string _enumJsonString, Action<string> action)
        {
            ZTHttpTool.Instance.Post(PlcKey.GetWriteMethodType(_SID), _enumJsonString, action);
        }
        
        void ClientWriteEnumDataFinsh(string _str)
        {
            Debug.Log("client write enum value call back ： " + _str);
            List<string> _strWriteEnumCallBack = _str.Split(new string[] { "\r\n" }, StringSplitOptions.None).ToList();
            _strWriteEnumCallBack = _strWriteEnumCallBack.Where(s => !string.IsNullOrEmpty(s)).ToList();

            if (_strWriteEnumCallBack[0] == "OK")
            {
                Debug.Log("Get Server Write enum callback !");
                //line 1 = ok ; line 2 = count;
                for (int i = 2; i < _strWriteEnumCallBack.Count; i++)
                {
                    // 0  = write enumValue to web server success ;
                    if (_strWriteEnumCallBack[i].Equals("0"))
                    {
                        Debug.Log("Threadtype : " + threadType.ToString() + " write enum value Type:" + enumListCache[i - 2].enumName + " success !");
                    }
                    else
                    {
                        //Debug.LogError("Threadtype : " + threadType.ToString() + " write enum value lose :" + _str);
                        Debug.LogError("Threadtype : " + threadType.ToString() + " data write callback :" + _str);
                    }
                }
            }
            else
            {
                InitWebServerEnumValueState = false;
                Debug.LogError("web server callback write error :" + _str);
            }
        }
        #endregion

        // private void Update()
        // {
        //     if (Input.GetKeyDown(KeyCode.Z))
        //     {
        //         WriteDataToWebServer(writeEnumList, "0");
        //     }
        // }

        private void InitWebServerEnumValue(List<EnumData> _enumList)
        {
            WriteDataToWebServer(_enumList, enumInitValue);
        }

        #region step 1 Write data to web server DataInit

        private void WriteDataToWebServer(List<EnumData> _enumList ,string _valueStr)
        {
            SetEnumList = true;
            enumListCache = _enumList;
            SplitEnumList(_enumList , _valueStr);
        }

        void SplitEnumList(List<EnumData> _enumList , string _valueStr)
        {
            List<EnumData> _splitList = new List<EnumData>();
            //一次发送的变量数量
            int _enumMaxCount = 10;
            int sleepTime = 5;
            for (int i = 0; i < _enumList.Count(); i++)
            {
                if (_splitList.Count() < _enumMaxCount && i < _enumList.Count() - 1)
                {
                    _splitList.Add(_enumList[i]);
                }
                else
                {
                    if (i == _enumList.Count() - 1)
                    {
                        _splitList.Add(_enumList[i]);
                    }
                    Thread.Sleep(100 * sleepTime);
                    var _str = SetWriteEnumJsonString(_splitList , _valueStr);
                    WriteEnumDataFromServer(sid, _str, WriteEnumDataFinsh);
                    _splitList.Clear();
                    _splitList.Add(_enumList[i]);
                    // Debug.LogError("count : + " + _enumList.Count + "number" + i + _splitList.Count());
                }
            }
            _splitList.Clear();
            InitWebServerEnumValueState = true;
            Thread.Sleep(1000 * sleepTime);
            //服务器默认全部写入都是成功的。1s写入十个变量，全部写完后，就默认开启变量监听。
            AutoReadEnumValueFromWebServer(enumListCache);
        }

        /// <summary>
        /// Set Http Request body data to json string
        /// </summary>
        /// <param name="_enumList">服务器获取的枚举变量列表</param>
        /// <param name="_valueStr">写入目标值</param>
        /// <returns></returns>
        private string SetWriteEnumJsonString(List<EnumData> _enumList,string _valueStr)
        {
            if (_enumList.Count == 0)
            {
                Debug.LogError("Enum value number is error");
                return "";
            }
            else
            {
                var _str = "";
                _str += _enumList.Count + "\r\n";
                //foreach (var item in _enumList)
                for (int i = 0; i < _enumList.Count; i++)
                {
                    _str += _enumList[i].enumName + "\r\n";
                    _str += _valueStr + "\r\n";
                    _enumList[i].value = _valueStr;
                }
                //Debug.LogError(enumListCache.Count + " : " + _str);
                return _str;
            }
        }

        private void WriteEnumDataFromServer(string _SID, string _enumJsonString,Action<string> action)
        {
            ZTHttpTool.Instance.Post(PlcKey.GetWriteMethodType(_SID), _enumJsonString, action);
        }

        void WriteEnumDataFinsh(string _str)
        {
            // Debug.Log(_str);
            List<string> _strWriteEnumCallBack = _str.Split(new string[] { "\r\n" }, StringSplitOptions.None).ToList();
            _strWriteEnumCallBack = _strWriteEnumCallBack.Where(s => !string.IsNullOrEmpty(s)).ToList();

            if (_strWriteEnumCallBack[0] == "OK")
            {
                Debug.Log("Get Server Write enum callback !");
                //line 1 = ok ; line 2 = count;
                for (int i = 2; i < _strWriteEnumCallBack.Count; i++)
                {
                    // 0  = write enumValue to web server success ;
                    if (_strWriteEnumCallBack[i].Equals("0"))
                    {
                        //Debug.Log("Threadtype : " + threadType.ToString() + " write enum value Type:" + enumListCache[i - 2].enumName + " success !");
                        //InitWebServerEnumValueState = true;
                    }
                    else
                    {
                        //InitWebServerEnumValueState = false;
                        //Debug.LogError("Threadtype : " + threadType.ToString() + " write enum value lose :" + _str);
                        Debug.LogError("Threadtype : " + threadType.ToString() + " data info :"+ _str);
                    }
                }
            }
            else
            {
                InitWebServerEnumValueState = false;
                Debug.LogError("web server callback write error :" + _str);
            }
        }

        public void AutoReadEnumValueFromWebServer(List<EnumData> _enumList)
        {
            if (!InitWebServerEnumValueState)
            {
                Debug.LogWarning("WebServer Enum Type Value not Init,Please ReSend init webserver enum value request ");
            }
            else
            {
                while (true)
                {
                    ReadDataFromWebServer();
                    Thread.Sleep(1200 * mreadDataFrequency);
                }
            }
        }

        /// <summary>
        /// reset webserver data  don't use
        /// </summary>
        /// <param name="_enumList"></param>
        private void ReSendWriteEnumValueData(List<EnumData> _enumList)
        {
            if (!InitWebServerEnumValueState)
            {
                Thread.Sleep(1000 * reSendWriteValueFrequency);
                InitWebServerEnumValue(_enumList);
                Debug.LogWarning("ReSend init webserver enum value request ");
            }
            else
            {
                //start to detected enum value is change 
                while (true)
                {
                    ReadDataFromWebServer();
                    Thread.Sleep(1200* mreadDataFrequency);
                }
            }
        }

        #endregion

        #region step 2 Read data from web Server 
        private void ReadDataFromWebServer()
        {
            if (!SetEnumList)
            {
                Debug.LogError("Please init Enumlist value from web server !");
                return;
            }
            var _str = GetEnumJsonString(enumListCache);
            ReadEnumDataFromServer(sid, _str);
        }

        private void ReadEnumDataFromServer(string _SID,string _enumJsonString)
        {
            ZTHttpTool.Instance.Post(PlcKey.GetReadMethodType(_SID), _enumJsonString, ReadEnumDataFinsh);
        }

        /// <summary>
        /// Get enumList To Json string for send read data request to web server
        /// </summary>
        /// <param name="_enumList"></param>
        /// <returns></returns>
        private string GetEnumJsonString(List<EnumData> _enumList)
        {
            if (_enumList.Count == 0)
            {
                Debug.LogError("Enum value number is error" + _enumList.Count());
                return "";
            }
            else
            {
                var _str = "";
                _str += _enumList.Count + "\r\n";
                //foreach (var item in _enumList)
                for (int i = 0; i < enumListCache.Count; i++)
                {
                    _str += enumListCache[i].enumName + "\r\n";
                }
                //Debug.LogError(_str);
                return _str;
            }
        }

        private bool plcConnectState = false;
        void ReadEnumDataFinsh(string _str)
        {
            List<string> _striparr = _str.Split(new string[] { "\r\n" },StringSplitOptions.None).ToList();
            _striparr = _striparr.Where(s => !string.IsNullOrEmpty(s)).ToList();
            Debug.Log("Read Enum data result 1 : " + _striparr[0] + "  2 : "  + _striparr[1] );
            //Debug.LogError("debug error 1 : " + _striparr[0]);
            if (_striparr[0].Equals("ERROR"))
            {
                Debug.LogError("设备断开链接：   等待设备重新链接 " + _str);
                return;
            }

            if (_striparr[0] == "OK")
            {
                if (isFirstToReadData)
                {
                    FirstToReadDataInit(_striparr);
                    stringCache = _str;
                    //测试全变量攻击 效果。
                    // WriteDataToWebServer(enumListCache, "0");
                }
                else
                {
                    //添加PLC掉线错误数据报告分类 
                    //Debug.LogError("设备断开链接：   2222" + _striparr[0]);
                    //Debug.LogError("设备断开链接：   2222" + _striparr[1].Equals("ERROR"));
                    DetectedPlcState(_striparr, _str);
                }
            }
            else
            {
                Debug.LogError(_str);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_striparr"></param>
        /// <param name="_str">server return data</param>
        public void DetectedPlcState(List<string> _striparr ,string  _str)
        {
            if (_striparr[2] == "#ERROR#7")
            {
                if (plcConnectState)
                {
                    plcConnectState = false;
                    RpcServer.Instance.serverSendMsg.SendPlcOffLineEventToClient(eSceneNameType,TeamName);
                }
            }
            else
            {
                //添加Plc重连服务器的消息处理。
                if (plcConnectState == false)
                {
                    plcConnectState = true;
                    RpcServer.Instance.serverSendMsg.SendPlcReconnectedEventToClient(eSceneNameType,TeamName);
                }
                EqualsReadDataRef(_str);   
            }
        }

        /// <summary>
        /// first To Read Data
        /// </summary>
        /// <param name="striparr"></param>
        void FirstToReadDataInit(List<string> striparr)
        {
            isFirstToReadData = false;
            plcConnectState = true;
            for (int i = 0; i < enumListCache.Count; i++)
            {
                //_striparr 0 = OK ;1 = count;
                enumListCache[i].value = striparr[i + 2];
                if (enumListCache[i].eSceneNameType == eSceneNameType)
                {
                    targetEnumCount++;
                }
                // Debug.Log(enumListCache[i].DebugSelf());
            }
        }

        /// <summary>
        /// 对比数据是否有更新
        /// </summary>
        /// <param name="_newStr">最新获取到的字符数据</param>
        void EqualsReadDataRef(string _newStr)
        {
            if (stringCache.Equals(_newStr))
            {
                Debug.Log("True has no data to ref : " + _newStr);
            }
            else
            {
                if (MatchGrmToSceneType)
                {
                    Debug.Log("false has data to ref : " + _newStr);
                    if (DetectedRefreshEnumIsAllAttack(_newStr, stringCache))
                    {
                        List<string> _strArryNew = _newStr.Split(new string[] { "\r\n" }, StringSplitOptions.None).ToList();
                        _strArryNew = _strArryNew.Where(s => !string.IsNullOrEmpty(s)).ToList();
                    
                        for (int i = 2; i < _strArryNew.Count; i++)
                        {
                            enumListCache[i - 2].value = _strArryNew[i];
                            Debug.Log("all attack data init : " + "Thread Type: " + threadType + " SceneType : " + enumListCache[i - 2].eSceneNameType
                                      + "  has data change :" + enumListCache[i - 2].enumName + " to :" + _strArryNew[i]);
                        }
                    }
                    else
                    {
                        RefreshReadData(_newStr, stringCache);
                    }
                }
                else
                {
                    RefreshReadData(_newStr, stringCache);
                }
            }
            stringCache = _newStr;
        }

        /// <summary>
        /// detected plc enum is all attack
        /// </summary>
        /// <param name="_newStr"></param>
        /// <param name="_cacheStr"></param>
        /// <returns></returns>
        public bool DetectedRefreshEnumIsAllAttack(string _newStr,string _cacheStr)
        {
            List<string> _newstriparr = _newStr.Split(new string[] { "\r\n" },StringSplitOptions.None).ToList();
            _newstriparr = _newstriparr.Where(s => !string.IsNullOrEmpty(s)).ToList();
            
            List<string> _cacheStriparr = _cacheStr.Split(new string[] { "\r\n" },StringSplitOptions.None).ToList();
            _cacheStriparr = _cacheStriparr.Where(s => !string.IsNullOrEmpty(s)).ToList();
            
            int _count = 0;
            for (int i = 2; i < _newstriparr.Count; i++)
            {
                if (enumListCache[i - 2].eSceneNameType == eSceneNameType && !_newstriparr[i].Equals(_cacheStriparr[i]))
                {
                    if (_newstriparr[i] != "1")
                        _count++;
                }
            }
            if (_count > (targetEnumCount - 1))
            {
                Debug.LogError("Refresh Enum count  : " +  _count + " all attack is true" + targetEnumCount + eSceneNameType.ToString());
                RpcServer.Instance.serverSendMsg.SendPlcAttackEventToClient(eSceneNameType,TeamName);
                return true;
            }
            else
            {
                Debug.LogError("Refresh Enum count  : " +  _count + " all attack is false"+ targetEnumCount + eSceneNameType.ToString());
                return false;
            }
        }

        /// <summary>
        /// Detected data refresh position
        /// </summary>
        /// <param name="_newStr"></param>
        /// <param name="_cacheStr"></param>
        void RefreshReadData(string _newStr,string _cacheStr)
        {
            Debug.Log("false has new data to ref : " + _newStr);
            List<string> _strArryNew = _newStr.Split(new string[] { "\r\n" }, StringSplitOptions.None).ToList();
            _strArryNew = _strArryNew.Where(s => !string.IsNullOrEmpty(s)).ToList();

            List<string> _strlistCache = _cacheStr.Split(new string[] { "\r\n" }, StringSplitOptions.None).ToList();
            _strlistCache = _strlistCache.Where(s => !string.IsNullOrEmpty(s)).ToList();

            for (int i = 0; i < _strArryNew.Count; i++)
            {
                if (!_strlistCache[i].Equals(_strArryNew[i]))
                {
                    //line 0 = OK; line 1 = count;
                    enumListCache[i - 2].value = _strArryNew[i];
                    Debug.Log("Thread Type: " + threadType.ToString() + " SceneType : " + enumListCache[i - 2].eSceneNameType.ToString() + "  has data change :" + enumListCache[i - 2].enumName + " From : " + _strlistCache[i] + " to :" + _strArryNew[i]);
                    // RpcServer.Instance.serverSendMsg.SendEnumValueChangeMsg(enumListCache[i - 2].eSceneNameType, enumListCache[i - 2].enumName, _strArryNew[i],threadType.ToString());
                    MatchGrmToScene(enumListCache[i - 2].eSceneNameType, enumListCache[i - 2].enumName, _strArryNew[i],threadType.ToString(),GrmNumber);
                }
            }
        }

        /// <summary>
        /// 是否根据grm 所属的场景进行数据分类分发
        /// </summary>
        /// <param name="_eSceneNameType"></param>
        /// <param name="_enumName"></param>
        /// <param name="_enumValue"></param>
        /// <param name="_threadType"></param>
        /// <param name="_grmNumber"></param>
        void MatchGrmToScene(ESceneNameType _eSceneNameType, string _enumName, string _enumValue,string _threadType,string _grmNumber)
        {
            if (!MatchGrmToSceneType)
            {
                RpcServer.Instance.serverSendMsg.SendEnumValueChangeMsg(_eSceneNameType, _enumName, _enumValue, TeamName,GrmNumber);
                Debug.LogError(" 不做场景匹配检测的消息转发");
                return;
            }
            else
            {
                // Debug.LogError(" 消息场景配对： 结果为：" + _eSceneNameType.ToString().Equals(eSceneNameType.ToString()));
                if (_eSceneNameType.ToString().Equals(eSceneNameType.ToString()))
                {
                    RpcServer.Instance.serverSendMsg.SendEnumValueChangeMsg(_eSceneNameType, _enumName, _enumValue, TeamName,GrmNumber);
                }
                else
                {
                    Debug.Log("No Match enumValue change Event :  SceneNameType : " + _eSceneNameType + "  enum Name : " + _enumName + "  enumValue : " + _enumValue);
                }
            }
        }

        #endregion
    }

}