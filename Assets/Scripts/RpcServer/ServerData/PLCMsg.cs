
using UnityEngine.Networking;
using UnityEngine;
using System.Runtime.InteropServices;
using System;
namespace Plc.Rpc
{
    /// <summary>
    /// 自定义信息类
    /// </summary>
    /// 
    [Obsolete]
    public class PLCMsg : MessageBase
    {
        public enum OrderType
        {
            None,
            GameState,
            Read,//webserver send enumValue change data to rpc client
            Write,//client send request rpcserver to webserver write enumValue
            ChangeDisplay,//rpcserver auto send jsonConfig to clients to init target display
            SetEnumType,//rpcserver request webserver Get enumtype .send to clients 
            PlcStateChange, // send plc State change event: off line  and reconnected  
            PlcAttack// Attack AllBreak event
        }
        public Rect rect_display;
        public ESceneNameType eSceneNameType = ESceneNameType.None;
        public OrderType orderType = OrderType.None;
        public string cmdEvent;
        public string enumName;
        public string enumValue;
        public string threadType;
        public string enumTypeList;
        public string teamName;
        public void SetUserMsg(ESceneNameType _eSceneNameType, OrderType _orderType, string _cmdEvent)
        {
            this.eSceneNameType = _eSceneNameType;
            this.orderType = _orderType;
            this.cmdEvent = _cmdEvent;
        }

        /// <summary>
        /// rpc server send display setting data to client 
        /// </summary>
        /// <param name="_rect"></param>
        public void SetChangeDisplayMsg(Rect _rect)
        {
            this.orderType = OrderType.ChangeDisplay;
            this.rect_display = _rect;
        }

        /// <summary>
        /// rpc server send enum value change to client
        /// </summary>
        /// <param name="_rect"></param>
        public void SetEnumValueChangeMsg(ESceneNameType _eSceneNameType, string _enumName, string _enumValue, string _threadType)
        {
            this.eSceneNameType = _eSceneNameType;
            this.orderType = OrderType.Read;
            this.enumName = _enumName;
            this.enumValue = _enumValue;
            this.threadType = _threadType;
        }

        /// <summary>
        /// client request to write enum type value 
        /// </summary>
        ///<param name="_enumTypeList">send client need to write enumtype list value</param>
        public void WriteEnumValueMsg(string _enumTypeList)
        {
            this.orderType = OrderType.Write;
            this.enumTypeList = _enumTypeList;
        }

        /// <summary>
        /// Set webserver enumtype list to string send to all rpc client;
        /// </summary>
        /// <param name="_enumTypeList">get webserver enumtype list</param>
        public void SetEnumTypeMsg(string _enumTypeList)
        {
            this.orderType = OrderType.SetEnumType;
            this.enumTypeList = _enumTypeList;
        }
        
        /// <summary>
        /// Send plc state change 
        /// </summary>
        /// <param name="_plcState">OffLine or Reconnected </param>
        public void SetPlcStateChangeMsg(ESceneNameType _eSceneNameType , string _plcState , string _teamName)
        {
            this.eSceneNameType = _eSceneNameType;
            this.orderType  = OrderType.PlcStateChange;
            this.cmdEvent = _plcState;
            this.teamName = _teamName;
        }

        /// <summary>
        /// send plc attack event to client : 1s AllBreak
        /// </summary>
        /// <param name="_plcAttackStr">AllBreak</param>
        public void SetPlcAttackMsg(ESceneNameType _eSceneNameType ,string _plcAttackStr, string _teamName)
        {
            this.eSceneNameType = _eSceneNameType;
            this.orderType  = OrderType.PlcAttack;
            this.cmdEvent = _plcAttackStr;
            this.teamName = _teamName;
        }
    }
}