using System.Collections;
using UnityEngine;
using ModbusTCP;
using System;
using System.Collections.Generic;
using Plc.Data;
using Plc.Rpc;
using Plc.WebServerRequest;

namespace Plc.ModbusTcp
{
    public partial class ModbusTcpClient : MonoBehaviour
    {
        [HideInInspector]
        public static ModbusTcpClient Instance;
        [HideInInspector]
        public Master MBmaster;
        private byte[] data;
        public string IP = "";
        public string Port = "";
        [SerializeField]
        public EThreadType threadType = EThreadType.None;
        private ItemVersionTwo item;
        public ItemVersionTwo Item { set => item = value; }

        public void SetThreadType(int _number)
        {
            threadType = SceneAndThreadsData.GetType(_number);
        }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                //ModbusInit();
            }
            
        }

        public void ModbusConnect(object obj)
        {
            try
            {
                ushort intPort = Convert.ToUInt16(Port);
                // Create new modbus master and add event functions
                MBmaster = new Master(IP, intPort);
                MBmaster.OnResponseData += new ModbusTCP.Master.ResponseData(MBmaster_OnResponseData);
                MBmaster.OnException += new ModbusTCP.Master.ExceptionData(MBmaster_OnException);
                
                ClientConnectedEvent();
            }
            catch (SystemException error)
            {
                Debug.Log(error.ToString());
            }
        }

        #region  enumType Data Init

        // List<ValueItem> ParseEnumType(ParsePlcEnumConfigJson parsePlcEnumConfigJson)
        // {
        //     switch (eSceneNameType)
        //     {
        //         case ESceneNameType.FirePower:
        //             return parsePlcEnumConfigJson.FirePower;
        //         case ESceneNameType.WindPower:
        //             return parsePlcEnumConfigJson.WindPower;
        //         case ESceneNameType.IntelligentManufacturing:
        //             return parsePlcEnumConfigJson.IntelligentManufacturing;
        //         case ESceneNameType.SolarPower:
        //             return parsePlcEnumConfigJson.SolarPower;
        //             break;
        //         case ESceneNameType.WarehouseLogistics:
        //             return parsePlcEnumConfigJson.WarehouseLogistics;
        //         case ESceneNameType.WaterPower:
        //             return parsePlcEnumConfigJson.WaterPower;
        //         case ESceneNameType.AutomobileMaking:
        //             return parsePlcEnumConfigJson.AutomobileMaking;
        //         case ESceneNameType.CoalToMethanol:
        //             return parsePlcEnumConfigJson.CoalToMethanol;
        //         case ESceneNameType.AviationOil:
        //             return parsePlcEnumConfigJson.AviationOil;
        //         default:
        //             return null;
        //     }
        // }

        List<EnumData> ParseEnumConfigList(List<ValueItem> _list, int[] _index)
        {
            List<EnumData> _enumList = new List<EnumData>();
            for (int i = 0; i < _list.Count; i++)
            {
                var _enumData = new EnumData();
                _enumData.sceneNumber = int.Parse(CalculateTools.MidStrEx(_list[i].ValueName, "S", "N"));
                _enumData.enumName = CalculateTools.GetStringName(_list[i].ValueName);
                if(_list[i].ValueName.Substring(_list[i].ValueName.Length - 1, 1) == "R") 
                {
                    _enumData.permissions = "ReadOnly";
                }
                else
                {
                    _enumData.permissions = "ReadAndWrite";
                }
                _enumData.value = initValue;
                _enumData.eSceneNameType = GetESceneType(i,_index);
                // _enumData.DebugSelf();
                _enumList.Add(_enumData);
            }
            return _enumList;
        }

        ESceneNameType GetESceneType(int _index, int[] _indexs)
        {
            if (_index < _indexs[0])
            {
                return ESceneNameType.FirePower;
            }
            else if(_index < _indexs[1])
            {
                return ESceneNameType.WindPower;
            }
            else if(_index < _indexs[2])
            {
                return ESceneNameType.IntelligentManufacturing;
            }
            else if(_index < _indexs[3])
            {
                return ESceneNameType.SolarPower;
            }
            else if(_index < _indexs[4])
            {
                return ESceneNameType.WarehouseLogistics;
            }
            else if(_index < _indexs[5])
            {
                return ESceneNameType.WaterPower;
            }
            else if(_index < _indexs[6])
            {
                return ESceneNameType.AutomobileMaking;
            }
            else if(_index < _indexs[7])
            {
                return ESceneNameType.CoalToMethanol;
            }
            else if(_index < _indexs[8])
            {
                return ESceneNameType.AviationOil;
            }
            else
            {
                return ESceneNameType.None;
            }
        }

        #endregion
        
        private void OnDestroy()
        {
            if (MBmaster != null)
            {
                MBmaster.Dispose();
                MBmaster = null;
            }
        }

        #region debug system info

        // ------------------------------------------------------------------------
        // Event for response data
        // ------------------------------------------------------------------------
        private void MBmaster_OnResponseData(ushort ID, byte function, byte[] values)
        {
            data = values;
            string str = "";

            for (int i = 0; i < data.Length; i++)
            {
                str += string.Format("{0:X2} ", data[i]);
            }
            // ------------------------------------------------------------------------
            // Identify requested data
            switch (ID)
            {
                case 1:
                    Debug.Log(str);
                    Debug.Log("获取开关通讯正常");
                    break;
                case 3:
                    // Debug.Log(str);
                    AutoReadEnumValueFinshEvent(str);
                    Debug.Log("读取寄存器通讯正常");
                    break;
                case 5:
                    Debug.Log(str);
                    Debug.Log("写入开关通讯正常");
                    break;
                case 6:
                    // Debug.Log(str);
                    WriteFinsh(str);
                    Debug.Log("写单路寄存器通讯正常");
                    break;
                case 16:
                    Debug.Log(str);
                    Debug.Log("多路写入通讯正常");
                    break;
            }
        }

        void WriteFinsh(string _str)
        {
            string[] b = _str.Split(' ');
            int firstAddress = 9;
            int writeValueIndex = 11;
            string index = string.Format("{0:X2} ", b[firstAddress]);
            string writeValueStr = string.Format("{0:X2} ", b[writeValueIndex]);
            // Debug.Log(  " write index value : " + index +"  write value : " + writeValueStr );

            if (!SetEnumList)
            {
                DetectedWriteValueInitState(index, writeValueStr);
            }

            // TypeEventSystem.Send(new ReceiveWriteMsg()
            // {
            //     
            // });
        }
        
        // ------------------------------------------------------------------------
        // Modbus TCP slave exception
        // ------------------------------------------------------------------------
        private void MBmaster_OnException(ushort id, byte function, byte exception)
        {
            switch (id)
            {
                case 1:
                    Debug.Log("获取开关通讯异常");
                    break;
                case 3:
                    Debug.Log("读取寄存器通讯异常");
                    break;
                case 5:
                    Debug.Log("写入开关通讯异常");
                    break;
                case 6:
                    Debug.Log("写单路寄存器通讯异常");
                    break;
                case 16:
                    Debug.Log("多路写入通讯异常");
                    break;
            }

            string exc = "Modbus says error: ";
            switch (exception)
            {
                case Master.excIllegalFunction: exc += "Illegal function!"; break;
                case Master.excIllegalDataAdr: exc += "Illegal data adress!"; break;
                case Master.excIllegalDataVal: exc += "Illegal data value!"; break;
                case Master.excSlaveDeviceFailure: exc += "Slave device failure!"; break;
                case Master.excAck: exc += "Acknoledge!"; break;
                case Master.excSlaveIsBusy: exc += "Slave is busy!"; break;
                case Master.excGatePathUnavailable: exc += "Gateway path unavailbale!"; break;
                case Master.excExceptionTimeout: exc += "Slave timed out!"; break;
                case Master.excExceptionConnectionLost: exc += "Connection is lost!"; break;
                case Master.excExceptionNotConnected: exc += "Not connected!"; break;
            }
            Debug.Log(exc + "Modbus slave exception");
        }

        #endregion
    }

}