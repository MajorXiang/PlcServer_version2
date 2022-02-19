using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Plc.Rpc
{
    public class ServerGetMsg : MonoBehaviour
    {
        [HideInInspector]
        public Dictionary<int, AddPortMsg> DportData = new Dictionary<int, AddPortMsg>();
        [HideInInspector]
        public event EventHandler newPortAdd;

        public void NewPortAddEvent()
        {
            if (newPortAdd != null)
                newPortAdd(this, EventArgs.Empty);
        }

        /// <summary>
        /// ServerGetAllMsg addprot and plcmsg data
        /// </summary>
        /// <param name="_netMsg">client send msg</param>
        [Obsolete]
        public void RpcServerGetMsg(NetworkMessage _netMsg)
        {
            if (IsContainsPort(_netMsg))
            {
                ServerGetMsgManage(_netMsg);
            }
            else
            {
                ServerAddPort(_netMsg);
            }
        }

        bool IsContainsPort(NetworkMessage _netMsg)
        {
            var _state = DportData.ContainsKey(_netMsg.conn.connectionId);
            return _state;
        }

        #region ServerGetMsgManage 
        void ServerGetMsgManage(NetworkMessage _netMsg)
        {
            PLCMsg _plcMsg = _netMsg.ReadMessage<PLCMsg>();
            Debug.Log("client : " + GetSceneType(_netMsg) + " Write : " + _plcMsg.enumTypeList);
            switch (_plcMsg.orderType)
            {
                case PLCMsg.OrderType.None:
                    break;
                case PLCMsg.OrderType.GameState:
                    break;
                case PLCMsg.OrderType.Read:
                    break;
                case PLCMsg.OrderType.Write:
                    RpcClientWriteEnumValue(_plcMsg);
                    break;
                case PLCMsg.OrderType.ChangeDisplay:
                    break;
                case PLCMsg.OrderType.SetEnumType:
                    break;
                default:
                    break;
            }
        }

        void RpcClientWriteEnumValue(PLCMsg _plcMsg)
        {
            //Debug.Log( " client send Write : " + _plcMsg.enumTypeList);
            RpcServer.Instance.webClientManage.WriteWebServerEnumValue(_plcMsg.enumTypeList);
        }

        /// <summary>
        /// get data eplatformType
        /// </summary>
        /// <param name="_netMsg">Client data </param>
        /// <returns></returns>
        ESceneNameType GetSceneType(NetworkMessage _netMsg)
        {
            return DportData[_netMsg.conn.connectionId].eplatformType;
        }
        #endregion


        /// <summary>
        /// client send Add prot data to server 
        ///  server auto to add client to DportData
        /// </summary>
        /// <param name="_netMsg">addport msg</param>
        void ServerAddPort(NetworkMessage _netMsg)
        {
            AddPortMsg _portData = _netMsg.ReadMessage<AddPortMsg>();
            _portData.id = _netMsg.conn.connectionId;
            _portData.address = _netMsg.conn.address;
            DportData.Add(_portData.id, _portData);
            _portData.DebugSelf("Add");
            //auto Send Set Display cmd
            RpcServer.Instance.serverSendMsg.SendSetDisplayMsg(_portData);
            //auto send enumtype list to all client;
            RpcServer.Instance.serverSendMsg.SendEnumTypeDataToClient(_portData);
        }

        /// <summary>
        /// auto to delete port 
        /// </summary>
        /// <param name="_netMsg"></param>
        public void ServerDeletePort(NetworkMessage _netMsg)
        {
            if (IsContainsPort(_netMsg))
            {
                AddPortMsg _portdata = DportData[_netMsg.conn.connectionId];
                _portdata.DebugSelf("Delete");
                DportData.Remove(_netMsg.conn.connectionId);
            }
            else
            {
                Debug.Log("未添加侦听使用的外部连接端口，不处理" + _netMsg.conn.address);
            }
        }
    }
}