
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Plc.WebServerRequest;
using Plc.Http;

namespace Plc.Rpc
{
    public class ServerSendMsg : MonoBehaviour
    {
        private RpcServer rpcServer;
        ClientsConfigJson clientsConfigJson;
        private List<EnumData> writeEnumList;
        private bool SetEnumList = false;
        private WebClientManage webClientManage;
        
        private void Awake()
        {
            Init();
        }

        void Init()
        {
            rpcServer = RpcServer.Instance;
            clientsConfigJson = new ParseClientsConfigJson().GetClientsConfigJson();
            webClientManage = RpcServer.Instance.webClientManage;
        }

        #region new port add ,client set display rect
        /// <summary>
        /// new port add to server 
        /// server auto to send set display msg to this port client
        /// </summary>
        /// <param name="_portData">new port</param>
        [System.Obsolete]
        public void SendSetDisplayMsg(AddPortMsg _portData)
        {
            int index = SelectDiplayItemIndex(_portData.eplatformType);
            PLCMsg _plcMsg = new PLCMsg();
            Rect _displayRect;
            _displayRect = GetClientDisplayRect(_portData, index);
            //if (clientsConfigJson.horizontal)//true 表示1 x 9  false 表示 3*3 
            //{
            //    _displayRect = GetClientDisplayRect(_portData, index);
            //}
            //else
            //{
            //    _displayRect = GetClientDisplayRect(_portData, index);
            //}
            _plcMsg.SetChangeDisplayMsg(_displayRect);
            rpcServer.SendToClientById(_portData.id, _plcMsg);
        }
        //屏幕传输
        //PLCMsg _plcMsg = new PLCMsg();
        //_plcMsg.SetChangeDisplayMsg(_displayRect);
        //rpcServer.SendToClientById(_portData.id, _plcMsg);


        //public void 

        /// <summary>
        /// get client exe play windows rect 
        /// </summary>
        /// <param name="_portData">new add port client data</param>
        /// <param name="_index">target jsondata display index</param>
        /// <returns></returns>
        Rect GetClientDisplayRect(AddPortMsg _portData , int _index)
        {
            Rect _rect = new Rect();
            _rect.width = clientsConfigJson.rectW;
            _rect.height = clientsConfigJson.rectH;
            int _targetDisplay = clientsConfigJson.diplay[_index].targetDisplayID -1;
            if (clientsConfigJson.horizontal)
            {
                _rect.x = _targetDisplay * _rect.width;
                _rect.y = 0;
            }
            else
            {
                _rect.x = 0;
                _rect.y = _targetDisplay * _rect.height;
            }
            return _rect;
        }

        /// <summary>
        /// from clientsConfigJson found ESceneNameType index
        /// </summary>
        /// <param name="_eSceneNameType">目标类型</param>
        /// <returns></returns>
        int SelectDiplayItemIndex(ESceneNameType _eSceneNameType)
        {
            for (int i = 0; i < clientsConfigJson.diplay.Count; i++)
            {
                Debug.Log(clientsConfigJson.diplay[i].eSceneNameType + " : " + _eSceneNameType);
                if (_eSceneNameType.ToString().Equals(clientsConfigJson.diplay[i].eSceneNameType))
                {
                    Debug.Log("Get index value : " + i);
                    return i;
                }
            }
            Debug.LogError("Get index value : null");
            return -1;
        }
        #endregion

        #region auto send webserver enum type to all rpc client
        /// <summary>
        /// when new client add to rpc server ,auto send enum type data to this client
        /// </summary>
        /// <param name="_portData">addPortData</param>
        public void SendEnumTypeDataToClient(AddPortMsg _portData)
        {
            GetEnumTypeListData();
            if (!SetEnumList)
                return;
            PLCMsg _plcMsg = new PLCMsg();
            var _enumList = new List<EnumData>();
            _enumList = GetSceneTypeEnumList(writeEnumList, _portData.eplatformType);
            var _enumStr = SetEnumListToString(_enumList);
            _plcMsg.SetEnumTypeMsg(_enumStr);
            //rpcServer.SendToAllClient(_plcMsg);
            rpcServer.SendToClientById(_portData.id, _plcMsg);
        }

        List<EnumData> GetSceneTypeEnumList(List<EnumData> _allWriteEnumList, ESceneNameType eplatformType)
        {
            var _enumList = new List<EnumData>();
            foreach (var item in _allWriteEnumList)
            {
                if (item.eSceneNameType == eplatformType)
                {
                    _enumList.Add(item);
                }
            }
            return _enumList;
        }

        void GetEnumTypeListData()
        {
            var _webclient = webClientManage.webClientList[0];
            if (!SetEnumList)
            {
                if (_webclient.SetEnumList)
                {
                    writeEnumList = _webclient.writeEnumList;
                    SetEnumList = true;
                }
                else
                {
                    Debug.LogError("webserver enumtype value is not ready !");
                }
            }
        }

        string SetEnumListToString(List<EnumData> _enumList)
        {
            var _str = "";
            //_str += _enumList.Count + "\r\n";
            for (int i = 0; i < _enumList.Count; i++)
            {
                _str += _enumList[i].enumName + "\r\n";
            }
            return _str;
        }

        #endregion

        #region auto refresh read data from webserver ,send data to client
        public void SendEnumValueChangeMsg(ESceneNameType _eSceneNameType, string _enumName, string _enumValue,string _threadType,string _grmNumber)
        {
            Debug.Log("_eSceneNameType : " + _eSceneNameType + "  _enumName : " + _enumName + " _enumValue change to value : " + _enumValue );
            
            PLCMsg _plcMsg = new PLCMsg();
            _plcMsg.SetEnumValueChangeMsg(_eSceneNameType,_enumName,_enumValue,_threadType);
            int _clientId = GetAddPortIdByEsceneType(_eSceneNameType);
            if (_clientId == -1)
            {
                return;
            }

            rpcServer.SendToClientById(_clientId, _plcMsg);//发送切换命令
            RpcServer.Instance.httpMgr.AutoSendAttackEvent(_plcMsg , _grmNumber);
            RpcServer.Instance.logRecord.SaveAttackEvent(_plcMsg);
        }
        

        /// <summary>
        /// from clientsConfigJson found ESceneNameType target client id
        /// </summary>
        /// <param name="_eSceneNameType">目标类型</param>
        /// <returns></returns>
        int GetAddPortIdByEsceneType(ESceneNameType _eSceneNameType)
        {
            var _DportData = RpcServer.Instance.serverGetMsg.DportData;
            //for (int i = 0; i < _DportData.Count; i++)
            foreach (var _addPortMsg in _DportData)
            {
                if (_addPortMsg.Value.eplatformType == _eSceneNameType)
                {
                    Debug.Log(_addPortMsg.Value.id);
                    return _addPortMsg.Value.id;
                }
            }
            Debug.LogError("Get iD value : null" + _eSceneNameType);
            return -1;
        }
        #endregion

        #region  Send Plc state change Msg to rpc client;

        public void SendPlcOffLineEventToClient(ESceneNameType _targetScene,string _teamName)
        {
            Debug.Log("PLC掉线，数据异常" + _targetScene);
            PLCMsg _plcMsg = new PLCMsg();
            _plcMsg.SetPlcStateChangeMsg(ESceneNameType.FirePower,"OffLine",_teamName);
            int _clientId = GetAddPortIdByEsceneType(_targetScene);
            if (_clientId == -1)
            {
                return;
            }
            rpcServer.SendToClientById(_clientId, _plcMsg);
        }
        
        public void SendPlcReconnectedEventToClient(ESceneNameType _targetScene,string _teamName)
        {
            Debug.Log("PLC重新连接" + _targetScene);
            PLCMsg _plcMsg = new PLCMsg();
            _plcMsg.SetPlcStateChangeMsg(ESceneNameType.FirePower,"Reconnected",_teamName);
            int _clientId = GetAddPortIdByEsceneType(_targetScene);
            if (_clientId == -1)
            {
                return;
            }
            rpcServer.SendToClientById(_clientId, _plcMsg);
        }
        
        public void SendPlcAttackEventToClient(ESceneNameType _targetScene,string _teamName)
        {
            Debug.Log("PLC 攻击全面攻占 " + _targetScene);
            PLCMsg _plcMsg = new PLCMsg();
            _plcMsg.SetPlcAttackMsg(ESceneNameType.FirePower,"AllBreak",_teamName);
            int _clientId = GetAddPortIdByEsceneType(_targetScene);
            if (_clientId == -1)
            {
                return;
            }
            rpcServer.SendToClientById(_clientId, _plcMsg);
        }
        
        #endregion
    }
}